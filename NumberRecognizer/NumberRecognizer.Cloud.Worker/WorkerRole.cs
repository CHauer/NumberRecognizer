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
                    }
                    catch
                    {
                        // Eventuelle Nachrichtenverarbeitungsausnahmen hier behandeln
                    }
                });

            completedEvent.WaitOne();
        }

        #endregion

    }
}