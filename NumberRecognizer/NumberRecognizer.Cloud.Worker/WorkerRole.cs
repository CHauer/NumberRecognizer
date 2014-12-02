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
        // Der Name Ihrer Warteschlange.
        const string QueueName = "ProcessingQueue";

        // QueueClient ist threadsicher. Es wird empfohlen, den QueueClient im Zwischenspeicher abzulegen 
        // statt ihn bei jeder Anforderung erneut zu erstellen
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Beginn der Verarbeitung von Nachrichten");

            // Initiiert das Nachrichtensystem und für jede erhaltene Nachricht wird der Rückruf aufgerufen; ein Aufruf von „Close“ auf dem Client beendet das Nachrichtensystem.
            Client.OnMessage((receivedMessage) =>
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

            CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
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
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Die Verbindung zur Service Bus-Warteschlange schließen
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}