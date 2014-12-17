//-----------------------------------------------------------------------
// <copyright file="WorkerRole.cs" company="FH Wr.Neustadt">
//     Copyright (c) Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>WorkerRole for Network Calculation.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Cloud.Worker
{
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

    /// <summary>
    /// The Azure Worker Role for calculating/training 
    /// new added of network.
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        /// <summary>
        /// The service bus queue name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// The queue client.
        /// </summary>
        private QueueClient client;

        /// <summary>
        /// The completed event
        /// </summary>
        private ManualResetEvent completedEvent = new ManualResetEvent(false);

        #region Worker Start/Stop

        /// <summary>
        /// Called when the worker role gets started.
        /// </summary>
        /// <returns>Role Status</returns>
        public override bool OnStart()
        {
            //get queuename from Settings
            this.QueueName = CloudConfigurationManager.GetSetting("QueueName");

            // Die maximale Anzahl gleichzeitiger Verbindungen setzen 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Die Warteschlange erstellen, falls sie noch nicht existiert
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(this.QueueName))
            {
                namespaceManager.CreateQueue(this.QueueName);
            }

            // Die Verbindung zur Service Bus-Warteschlange initialisieren
            this.client = QueueClient.CreateFromConnectionString(connectionString, this.QueueName);

            return base.OnStart();
        }

        /// <summary>
        /// Called when the worker role is stop.
        /// </summary>
        public override void OnStop()
        {
            // Die Verbindung zur Service Bus-Warteschlange schließen
            this.client.Close();
            this.completedEvent.Set();
            base.OnStop();
        }

        #endregion

        #region Run - Message Handling

        /// <summary>
        /// Runs this worker role instance.
        /// Handles incoming messages from service bus queue.
        /// </summary>
        public override void Run()
        {
            Trace.WriteLine("Beginn der Verarbeitung von Nachrichten");

            // Initiiert das Nachrichtensystem und für jede erhaltene Nachricht wird der Rückruf aufgerufen;
            // ein Aufruf von „Close“ auf dem Client beendet das Nachrichtensystem.
            this.client.OnMessage((receivedMessage) =>
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
                        else if (message.Equals("trainRenew"))
                        {
                            TrainNetwork(Convert.ToInt32(receivedMessage.Properties["networkId"]), true);
                        }
                        else
                        {
                            //Warning unknown message to handle
                            Trace.TraceWarning(String.Format("Unknown Message {0}", message));
                        }

                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                });

            this.completedEvent.WaitOne();
        }

        /// <summary>
        /// Trains the network with the received network id.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="reTrainNetwork">if set to <c>true</c> the network is prepared 
        ///                              for repeating the training.</param>
        private void TrainNetwork(int networkId, bool reTrainNetwork = false)
        {
            NetworkDataSerializer networkDataSerializer = new NetworkDataSerializer();
            DataSerializer<double[,]> arrayDataSerializer = new DataSerializer<double[,]>();

            NetworkTrainer trainer;

            using (var db = new NetworkDataModelContainer())
            {
                //check if network exists
                if (!db.NetworkSet.Any(n => n.NetworkId == networkId))
                {
                    Trace.TraceError("The network (ID:{0}) was not found in the database.");
                    return;
                }

                //load network entity
                var dbNetwork = db.NetworkSet.First(n => n.NetworkId == networkId);

                if (reTrainNetwork)
                {
                    //prepare network for new training (delete logs)
                    if (!this.PrepareNetworkForReTraining(networkId, dbNetwork, db)) return;
                }
                else
                {
                    //check if network is trained already
                    if (dbNetwork.Calculated == CalculationType.Running || dbNetwork.Calculated == CalculationType.Ready)
                    {
                        Trace.TraceError("The network (ID:{0}) is already trained - use retrain function to train this network again.");
                        return;
                    }
                }

                //load training data
                List<TrainingImageData> trainImages = dbNetwork.TrainingImages.ToList();

                //transform to double arrays
                IEnumerable<PatternTrainingImage> trainData = trainImages.Select(i => new PatternTrainingImage()
                {
                    PixelValues = arrayDataSerializer.TransformFromBinary(i.ImageData),
                    RepresentingInformation = i.Pattern
                });

                //Create network trainer instance
                trainer = new NetworkTrainer(trainData);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

                dbNetwork.CalculationStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
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

            trainer.MultipleGenPoolGenerationChanged += (sender, e) => this.Trainer_GenerationChanged(sender, e, networkId);
            trainer.GenerationChanged += (sender, e) => this.Trainer_GenerationChanged(sender, e, networkId);

            //Train the network and save final network instance
            PatternRecognitionNetwork finalNetwork = trainer.TrainNetwork().First();

            using (var db = new NetworkDataModelContainer())
            {
                var dbNetwork = db.NetworkSet.First(n => n.NetworkId == networkId);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

                dbNetwork.NetworkData = networkDataSerializer.TransformToBinary(finalNetwork);
                dbNetwork.CalculationEnd = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
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
        /// Prepares the network for re training.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="dbNetwork">The database network.</param>
        /// <param name="db">The database.</param>
        /// <returns>Role Status</returns>
        private bool PrepareNetworkForReTraining(int networkId, Network dbNetwork, NetworkDataModelContainer db)
        {
            if (dbNetwork.Calculated == CalculationType.Ready)
            {
                //remove old patterns fittness values
                var removePatterns = db.PatternFitnessSet.Where(p => p.TrainLog.Network.NetworkId == networkId);
                db.PatternFitnessSet.RemoveRange(removePatterns);

                //remove old trainlog
                var removeTrainLogs = dbNetwork.TrainLogs;
                db.TrainLogSet.RemoveRange(removeTrainLogs);

                dbNetwork.Calculated = CalculationType.NotStarted;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    return false;
                }
            }
            else if (dbNetwork.Calculated == CalculationType.Running)
            {
                Trace.TraceError("The trainig for the network (ID:{0}) has not finished.", networkId);
                return false;
            }

            return true;
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
            NetworkDataSerializer networkDataSerializer = new NetworkDataSerializer();
            string genPool = null;

            if (e is MultipleGenPoolGenerationChangedEventArgs)
            {
                genPool = (e as MultipleGenPoolGenerationChangedEventArgs).MultipleGenPoolIdentifier.ToString();
            }

            using (var db = new NetworkDataModelContainer())
            {
                var dbNetwork = db.NetworkSet.First(n => n.NetworkId == networkId);

                dbNetwork.Fitness = e.CurrentFittestNetwork.Fitness;
                dbNetwork.NetworkData = networkDataSerializer.TransformToBinary(e.CurrentFittestNetwork);

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