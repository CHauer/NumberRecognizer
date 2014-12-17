//-----------------------------------------------------------------------
// <copyright file="NumberRecognizerService.svc.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>NumberRecognizerService.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Cloud.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using NumberRecognizer.Cloud.Contract;
    using NumberRecognizer.Cloud.Contract.Data;
    using NumberRecognizer.Cloud.Data;
    using NumberRecognizer.Lib.DataManagement;
    using NumberRecognizer.Lib.Network;
    using NumberRecognizer.Cloud.Contract.Data;

    /// <summary>
    /// NumberRecognizerService - Service Role.
    /// </summary>
    public class NumberRecognizerService : INumberRecognizerService
    {
        /// <summary>
        /// The queue client
        /// </summary>
        private QueueClient queueClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberRecognizerService"/> class.
        /// </summary>
        public NumberRecognizerService()
        {
            InitializeServiceBusQueue();
        }

        #region Initialize

        /// <summary>
        /// Initializes the service bus queue.
        /// </summary>
        private void InitializeServiceBusQueue()
        {
            string queueName = CloudConfigurationManager.GetSetting("QueueName");

            // Die maximale Anzahl gleichzeitiger Verbindungen setzen 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Die Warteschlange erstellen, falls sie noch nicht existiert
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(queueName);
            }

            queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName);
        }

        #endregion

        /// <summary>
        /// Gets the networks.
        /// </summary>
        /// <returns>A list of current available networks.</returns>
        public IList<NetworkInfo> GetNetworks()
        {
            List<NetworkInfo> networks;

            using (var db = new NetworkDataModelContainer())
            {
                //load networks
                networks = db.NetworkSet.Select(network => new NetworkInfo()
                {
                    Calculated = network.Calculated == CalculationType.Ready,
                    Status = (NetworkStatusType)((int)network.Calculated),
                    NetworkFitness = network.Fitness,
                    NetworkId = network.NetworkId,
                    NetworkName = network.NetworkName,
                    CalculationStart = network.CalculationStart,
                    CalculationEnd = network.CalculationEnd,

                }).ToList();

                foreach (var network in networks.Where(network => network.Calculated))
                {
                    var dbNetwork = db.NetworkSet.First(n => n.NetworkId == network.NetworkId);

                    int maxGeneration = dbNetwork.TrainLogs
                                        .Where(tl => tl.MultipleGenPoolIdentifier == null)
                                        .Max(n => n.GenerationNr);
                    try
                    {
                        network.FinalPatternFittness = dbNetwork.TrainLogs
                                .Where(tl => tl.MultipleGenPoolIdentifier == null)
                                .First(tl => tl.GenerationNr == maxGeneration)
                                .PatternFitnessSet.ToDictionary(i => i.Pattern, i => i.Fitness);

                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
            }

            return networks;
        }

        /// <summary>
        /// Gets the network detail.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns>
        /// A current available network.
        /// </returns>
        public NetworkInfo GetNetworkDetail(int networkId)
        {
            NetworkInfo result = null;

            using (var db = new NetworkDataModelContainer())
            {
                var detailNetwork = db.NetworkSet.FirstOrDefault(n => n.NetworkId == networkId);

                if (detailNetwork == null)
                {
                    throw new FaultException<ArgumentException>(new ArgumentException(String.Format("The network with the ID {0} was not found!", networkId)));
                }

                int maxGeneration = detailNetwork.TrainLogs
                                       .Where(tl => tl.MultipleGenPoolIdentifier == null)
                                       .Max(n => n.GenerationNr);

                result = new NetworkInfo()
                {
                    Calculated = detailNetwork.Calculated == CalculationType.Ready,
                    Status = (NetworkStatusType)((int)detailNetwork.Calculated),
                    NetworkFitness = detailNetwork.Fitness,
                    NetworkId = detailNetwork.NetworkId,
                    NetworkName = detailNetwork.NetworkName,
                    CalculationStart = detailNetwork.CalculationStart,
                    CalculationEnd = detailNetwork.CalculationEnd
                };

                try
                {
                    result.FinalPatternFittness = detailNetwork.TrainLogs
                                .Where(tl => tl.MultipleGenPoolIdentifier == null)
                                .First(tl => tl.GenerationNr == maxGeneration)
                                .PatternFitnessSet.ToDictionary(i => i.Pattern, i => i.Fitness);

                    result.FinalPoolFitnessLog = GetFitnessLog(result.NetworkId, null, db);
                    result.MultiplePoolFitnessLog = GetMultiplePoolFitnessLog(result.NetworkId, db);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates the network.
        /// </summary>
        /// <param name="networkName">Name of the network.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <returns>
        /// Status Flag.
        /// </returns>
        public bool CreateNetwork(string networkName, IEnumerable<TrainingImage> individualTrainingsData)
        {
            bool status = true;
            DataSerializer<double[,]> serializer = new DataSerializer<double[,]>();
            int networkId = -1;

            using (var db = new NetworkDataModelContainer())
            {
                var newNetwork = db.NetworkSet.Add(new Network()
                {
                    NetworkName = networkName,
                    TrainingImages = new List<TrainingImageData>(),
                    Calculated = CalculationType.NotStarted
                });

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    status = false;
                }

                //update the local copy of new created entity with database values
                db.Entry(newNetwork).GetDatabaseValues();
                networkId = newNetwork.NetworkId;

                var newTrainigData = new List<TrainingImageData>();
                individualTrainingsData.ToList().ForEach(data => newTrainigData.Add(new TrainingImageData()
                {
                    Pattern = data.Pattern,
                    ImageData = serializer.TransformToBinary(data.TransformTo2DArrayFromImageData()),
                    Network = newNetwork
                }));

                db.TrainingImageDataSet.AddRange(newTrainigData);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    status = false;
                }
            }

            BrokeredMessage msg = new BrokeredMessage("training");
            msg.Properties["networkId"] = networkId;

            try
            {
                queueClient.Send(msg);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Creates the network and copies the training data from a previous network.
        /// </summary>
        /// <param name="networkName">Name of the network.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <param name="copyTraindataFromNetworkId">The copy training data from network identifier.</param>
        /// <returns>
        /// Status
        /// </returns>
        public bool CreateNetworkWithTrainingDataCopy(string networkName, IEnumerable<TrainingImage> individualTrainingsData, int copyTraindataFromNetworkId)
        {
            bool status = true;
            DataSerializer<double[,]> serializer = new DataSerializer<double[,]>();
            int networkId = -1;

            using (var db = new NetworkDataModelContainer())
            {
                var newNetwork = db.NetworkSet.Add(new Network()
                {
                    NetworkName = networkName,
                    TrainingImages = new List<TrainingImageData>(),
                    Calculated = CalculationType.NotStarted
                });

                var copyNetwork = db.NetworkSet.First(n => n.NetworkId == copyTraindataFromNetworkId);

                networkId = newNetwork.NetworkId;

                var newTrainigData = new List<TrainingImageData>();
                individualTrainingsData.ToList().ForEach(data => newTrainigData.Add(new TrainingImageData()
                {
                    Pattern = data.Pattern,
                    ImageData = serializer.TransformToBinary(data.TransformTo2DArrayFromImageData()),
                    Network = newNetwork
                }));

                db.TrainingImageDataSet.AddRange(newTrainigData);
                db.TrainingImageDataSet.AddRange(copyNetwork.TrainingImages);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    status = false;
                }
            }

            BrokeredMessage msg = new BrokeredMessage("training");
            msg.Properties["networkId"] = networkId;

            try
            {
                queueClient.Send(msg);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Deletes the network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns>
        /// Status
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public bool DeleteNetwork(int networkId)
        {
            using (var db = new NetworkDataModelContainer())
            {
                var delNetwork = db.NetworkSet.FirstOrDefault(n => n.NetworkId == networkId);

                if (delNetwork == null)
                {
                    throw new FaultException<ArgumentException>(new ArgumentException(String.Format("The network with the ID {0} was not found!", networkId)));
                }

                db.PatternFitnessSet.RemoveRange(db.PatternFitnessSet.Where(pfs => pfs.TrainLog.Network.NetworkId == networkId));
                db.TrainingImageDataSet.RemoveRange(delNetwork.TrainingImages);
                db.TrainLogSet.RemoveRange(delNetwork.TrainLogs);
                db.NetworkSet.Remove(delNetwork);

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

            return true;
        }

        /// <summary>
        /// Res the train network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns>
        /// Status
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public bool ReTrainNetwork(int networkId)
        {
            bool status = true;

            using (var db = new NetworkDataModelContainer())
            {
                var delNetwork = db.NetworkSet.FirstOrDefault(n => n.NetworkId == networkId);

                if (delNetwork == null)
                {
                    throw new FaultException<ArgumentException>(new ArgumentException(String.Format("The network with the ID {0} was not found!", networkId)));
                }
            }

            BrokeredMessage msg = new BrokeredMessage("trainRenew");
            msg.Properties["networkId"] = networkId;

            try
            {
                queueClient.Send(msg);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Recognizes the phone number from image.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="imageData">The image data.</param>
        /// <returns>
        /// Recognition Result.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public NumberRecognitionResult RecognizePhoneNumber(int networkId, IList<RecognitionImage> imageData)
        {
            NetworkDataSerializer dataSerializer = new NetworkDataSerializer();
            PatternRecognitionNetwork patternRecognitionNetwork;
            StringBuilder numberBuilder = new StringBuilder();
            NumberRecognitionResult result = new NumberRecognitionResult
            {
                Items = new List<NumberRecognitionResultItem>()
            };

            using (var db = new NetworkDataModelContainer())
            {
                var networkFromDb = db.NetworkSet.FirstOrDefault(n => n.NetworkId == networkId);

                if (networkFromDb == null)
                {
                    throw new FaultException<ArgumentException>(new ArgumentException(String.Format("The network with the ID {0} was not found!", networkId)));
                }

                patternRecognitionNetwork = dataSerializer.TransformFromBinary(networkFromDb.NetworkData);
            }

            foreach (RecognitionImage image in imageData)
            {
                var recognitionResult = patternRecognitionNetwork.RecognizeCharacter(image.TransformTo2DArrayFromImageData()).ToList();

                numberBuilder.Append(recognitionResult[0].RecognizedCharacter);

                result.Items.Add(new NumberRecognitionResultItem()
                {
                    NumericCharacter = Convert.ToInt32(recognitionResult[0].RecognizedCharacter),
                    Probabilities = recognitionResult.ToDictionary(key => key.RecognizedCharacter[0], value => value.Propability)
                });
            }

            result.Number = numberBuilder.ToString();

            return result;
        }

        /// <summary>
        /// Gets the multiple pool fitness log.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="db">The database.</param>
        /// <returns>
        /// FitnessLog.
        /// </returns>
        private Dictionary<string, FitnessLog> GetMultiplePoolFitnessLog(int networkId, NetworkDataModelContainer db)
        {
            var network = db.NetworkSet.First(n => n.NetworkId == networkId);

            if (network.TrainLogs.Count == 0)
            {
                return null;
            }

            var pools = network.TrainLogs
                .Where(tl => tl.MultipleGenPoolIdentifier != null)
                .Select(tl => tl.MultipleGenPoolIdentifier).Distinct();

            return pools.ToDictionary(poolId => poolId, poolId => GetFitnessLog(networkId, poolId, db));
        }

        /// <summary>
        /// Gets the fitness log.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="poolId">The pool identifier.</param>
        /// <param name="db">The database.</param>
        /// <returns>FitnessLog</returns>
        private FitnessLog GetFitnessLog(int networkId, string poolId, NetworkDataModelContainer db)
        {
            //load network
            var network = db.NetworkSet.First(n => n.NetworkId == networkId);

            if (network == null || network.TrainLogs.Count == 0)
            {
                //return empty standard value
                return new FitnessLog();
            }

            Func<TrainLog, bool> funcPoolCompare = tl =>
            {
                if (String.IsNullOrEmpty(poolId))
                {
                    return tl.MultipleGenPoolIdentifier == null;
                }

                if (!String.IsNullOrEmpty(tl.MultipleGenPoolIdentifier))
                {
                    return tl.MultipleGenPoolIdentifier.Equals(poolId);
                }

                return false;
            };

            try
            {
                return new FitnessLog()
                {
                    FitnessTrend = network.TrainLogs
                        .Where(funcPoolCompare)
                        .OrderBy(tl => tl.GenerationNr)
                        .Select(tl => tl.Fitness)
                        .ToList<double>(),
                };
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);

                //return empty standard value
                return new FitnessLog();
            }
        }
    }
}
