using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using NumberRecognizer.Cloud.Data;
using NumberRecognizer.Lib.DataManagement;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training;
using NumberRecognizer.Lib.Training.Events;

namespace NumberRecognizer.Cloud.Worker
{
    public class WorkerRole : RoleEntryPoint
    {

        /// <summary>
        /// Der Name Ihrer Warteschlange.
        /// The queue name
        /// </summary>
        public string QueueName { get; set; }

        // QueueClient ist threadsicher. Es wird empfohlen, den QueueClient im Zwischenspeicher abzulegen 
        // statt ihn bei jeder Anforderung erneut zu erstellen
        /// <summary>
        /// The client
        /// </summary>
        private QueueClient client;

        /// <summary>
        /// The completed event
        /// </summary>
        private ManualResetEvent completedEvent = new ManualResetEvent(false);

        #region Initialize

        /// <summary>
        /// Called when [start].
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            QueueName = CloudConfigurationManager.GetSetting("QueueName");

            // Die maximale Anzahl gleichzeitiger Verbindungen setzen 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Die Warteschlange erstellen, falls sie noch nicht existiert
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Die Verbindung zur Service Bus-Warteschlange initialisieren
            client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        /// <summary>
        /// Called when [stop].
        /// </summary>
        public override void OnStop()
        {
            // Die Verbindung zur Service Bus-Warteschlange schließen
            client.Close();
            completedEvent.Set();
            base.OnStop();
        }

        #endregion

        #region Run - Message Handling

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public override void Run()
        {
            Trace.WriteLine("Beginn der Verarbeitung von Nachrichten");

            // Initiiert das Nachrichtensystem und für jede erhaltene Nachricht wird der Rückruf aufgerufen; ein Aufruf von „Close“ auf dem Client beendet das Nachrichtensystem.
            client.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        // Die Nachricht verarbeiten
                        Trace.WriteLine("Verarbeiten der Servicebus-Nachricht: " + receivedMessage.SequenceNumber.ToString());

                        string message = receivedMessage.GetBody<string>();

                        if (message.Equals("training"))
                        {
                            TrainNetwork(Convert.ToInt32(receivedMessage.Properties["networkId"]));
                        }
                        else
                        {
                            Trace.TraceWarning(String.Format("Unknown Message {0}", message));
                        }

                    }
                    catch(Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                });

            completedEvent.WaitOne();
        }

        /// <summary>
        /// Trains the network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        private void TrainNetwork(int networkId)
        {
            NetworkDataManager networkDataManager = new NetworkDataManager();
            DataManager<double[,]> arrayDataManager = new DataManager<double[,]>();

            PatternRecognitionNetwork network;
            NetworkTrainer trainer;

            using(var db = new NetworkDataModelContainer())
            {
                var dbNetwork = db.NetworkSet.First(n => n.NetworkId == networkId);
                var trainingData = dbNetwork.TrainingImages.Select(i => new PatternTrainingImage()
                {
                    PixelValues = arrayDataManager.TransformFromBinary(i.ImageData),
                    RepresentingInformation = i.Pattern
                });

                trainer = new NetworkTrainer(trainingData);

                dbNetwork.CalculationStart = DateTime.Now;
                dbNetwork.Calculated = CalculationType.Running;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }

            trainer.MultipleGenPoolGenerationChanged += (sender, e) => Trainer_GenerationChanged(sender, e, networkId);

            trainer.GenerationChanged += (sender, e) => Trainer_GenerationChanged(sender, e, networkId);

            PatternRecognitionNetwork finalNetwork = trainer.TrainNetwork().First();

            using (var db = new NetworkDataModelContainer())
            {
                var dbNetwork = db.NetworkSet.First(n => n.NetworkId == networkId);

                dbNetwork.NetworkData = networkDataManager.TransformToBinary(finalNetwork);
                dbNetwork.CalculationEnd = DateTime.Now;
                dbNetwork.Calculated = CalculationType.Ready;
                dbNetwork.Fitness = finalNetwork.Fitness;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }

        }

        /// <summary>
        /// Trainer generation changed event handler.
        /// Save a new train log entry to the database.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GenerationChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="networkId">The network identifier.</param>
        private void Trainer_GenerationChanged(object sender, GenerationChangedEventArgs e, int networkId)
        {
            string genPool = null;

            if (e is MultipleGenPoolGenerationChangedEventArgs)
            {
                genPool = (e as MultipleGenPoolGenerationChangedEventArgs).MultipleGenPoolIdentifier.ToString();
            }

            using (var db = new NetworkDataModelContainer())
            {
                var dbNetwork = db.NetworkSet.First(n => n.NetworkId == networkId);

                var newTrainLog = db.TrainLogSet.Add(new TrainLog()
                {
                    Fitness = e.CurrentFittestNetwork.Fitness,
                    GenerationNr = e.Generation,
                    MultipleGenPoolIdentifier = genPool,
                    Network = dbNetwork
                });

                db.PatternFitnessSet.AddRange(e.PatternFitness.Keys.Select(key => new PatternFitness()
                {
                    Fitness = e.PatternFitness[key],
                    Pattern = key,
                    TrainLog = newTrainLog
                }));

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
        }

        #endregion

    }
}