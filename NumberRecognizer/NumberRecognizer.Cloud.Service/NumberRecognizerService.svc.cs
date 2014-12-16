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

namespace NumberRecognizer.Cloud.Service
{
    /// <summary>
    /// 
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
                    CalculationEnd = network.CalculationEnd
                }).ToList();

                //load trainlog data 
                foreach (var network in networks.Where(network => network.Calculated))
                {
                    try
                    {
                        network.FinalPoolFitnessLog = GetFitnessLog(network.NetworkId, null, db);
                        network.MultiplePoolFitnessLog = GetMultiplePoolFitnessLog(network.NetworkId, db);
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
        /// Creates the network.
        /// </summary>
        /// <param name="networkName">Name of the network.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <returns></returns>
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
        /// <param name="copyTraindataFromNetworkId">The copy traindata from network identifier.</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public bool DeleteNetwork(int networkId)
        {
            using (var db = new NetworkDataModelContainer())
            {
                var delNetwork = db.NetworkSet.FirstOrDefault(n => n.NetworkId == networkId);

                if (delNetwork == null)
                {
                    throw new FaultException<ArgumentException>
                        (new ArgumentException(String.Format("The network with the ID {0} was not found!", networkId)));
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
        /// Recognizes the phone number from image.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="imageData">The image data.</param>
        /// <returns></returns>
        public NumberRecognitionResult RecognizePhoneNumber(int networkId, IList<RecognitionImage> imageData)
        {
            NetworkDataSerializer dataSerializer = new NetworkDataSerializer();
            PatternRecognitionNetwork network;
            StringBuilder numberBuilder = new StringBuilder();
            NumberRecognitionResult result = new NumberRecognitionResult
            {
                Items = new List<Contract.Data.NumberRecognitionResultItem>()
            };

            using (var db = new NetworkDataModelContainer())
            {
                var networkFromDb = db.NetworkSet.First(n => n.NetworkId == networkId);

                network = dataSerializer.TransformFromBinary(networkFromDb.NetworkData);
            }

            foreach (RecognitionImage image in imageData)
            {
                var recognitionResult = network.RecognizeCharacter(image.TransformTo2DArrayFromImageData()).ToList();

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
        /// <returns></returns>
        private Dictionary<string, FitnessLog> GetMultiplePoolFitnessLog(int networkId, NetworkDataModelContainer db)
        {
            Dictionary<string, FitnessLog> poolLogs = new Dictionary<string, FitnessLog>();

            var network = db.NetworkSet.First(n => n.NetworkId == networkId);

            if (network.TrainLogs.Count == 0)
            {
                return null;
            }

            var pools = network.TrainLogs.Where(tl => tl.MultipleGenPoolIdentifier != null).Select(tl => tl.MultipleGenPoolIdentifier).Distinct();

            foreach (string poolId in pools)
            {
                poolLogs.Add(poolId, GetFitnessLog(networkId, poolId, db));
            }

            return poolLogs;
        }

        /// <summary>
        /// Gets the fitness log.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="poolId">The pool identifier.</param>
        /// <returns></returns>
        private FitnessLog GetFitnessLog(int networkId, string poolId, NetworkDataModelContainer db)
        {
            //Standard value
            int maxGeneration = 0;

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

                return tl.MultipleGenPoolIdentifier.Equals(poolId);
            };

            Func<PatternFitness, bool> funcPatternPoolCompare = pf =>
            {
                if (String.IsNullOrEmpty(poolId))
                {
                    return pf.TrainLog.MultipleGenPoolIdentifier == null;
                }

                return pf.TrainLog.MultipleGenPoolIdentifier.Equals(poolId);
            };

            //load max training generation of network
            maxGeneration = network.TrainLogs
                                     .Where(funcPoolCompare)
                                     .Max(n => n.GenerationNr);

            //load distinct patterns
            List<string> patterns = network.TrainLogs.First()
                                           .PatternFitnessSet
                                           .Select(p => p.Pattern)
                                           .Distinct()
                                           .ToList();

            try
            {
                return new FitnessLog()
                {
                    FitnessTrend = network.TrainLogs
                        .Where(funcPoolCompare)
                        .OrderBy(tl => tl.GenerationNr)
                        .Select(tl => tl.Fitness)
                        .ToList<double>(),

                    FinalPatternFittness = network.TrainLogs
                        .Where(funcPoolCompare)
                        .First(tl => tl.GenerationNr == maxGeneration)
                        .PatternFitnessSet.ToDictionary(i => i.Pattern, i => i.Fitness),

                    PatternTrends = patterns.ToDictionary(key => key,
                        value => db.PatternFitnessSet
                                .Where(p => p.TrainLog.Network.NetworkId == networkId)
                                .Where(funcPatternPoolCompare)
                                .Where(p => p.Pattern.Equals(value))
                                .OrderBy(i => i.TrainLog.GenerationNr)
                                .Select(i => i.Fitness)
                                .ToList<double>())
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
