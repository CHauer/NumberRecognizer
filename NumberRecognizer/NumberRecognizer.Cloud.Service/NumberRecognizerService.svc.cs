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
    public class NumberRecognizerService :  INumberRecognizerService
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
        /// <returns></returns>
        public IList<NetworkInfo> GetNetworks()
        {
            using (var db = new NetworkDataModelContainer())
            {
                var networks = db.NetworkSet.Select(network => new NetworkInfo()
                {
                    Calculated = network.Calculated == CalculationType.Ready ? true : false,
                    NetworkFitness = network.Fitness,
                    NetworkId = network.NetworkId,
                    NetworkName = network.NetworkName,
                    CalculationStart = network.CalculationStart,
                    CalculationEnd = network.CalculationEnd,
                    FinalPoolFitnessLog = GetFitnessLog(network.NetworkId, null),
                    MultiplePoolFitnessLog = GetMultiplePoolFitnessLog(network.NetworkId)
                });

                return networks.ToList();
            }

            
        }

        /// <summary>
        /// Gets the multiple pool fitness log.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns></returns>
        private Dictionary<string, FitnessLog> GetMultiplePoolFitnessLog(int networkId)
        {
            Dictionary<string, FitnessLog> poolLogs = new Dictionary<string, FitnessLog>();

            using (var db = new NetworkDataModelContainer())
            {
                var network = db.NetworkSet.First(n => n.NetworkId == networkId);
                var pools = network.TrainLogs.Select(tl => tl.MultipleGenPoolIdentifier).Distinct();

                foreach (string poolId in pools)
                {
                    poolLogs.Add(poolId, GetFitnessLog(networkId, poolId));
                }
            }

            return poolLogs;
        }

        /// <summary>
        /// Gets the fitness log.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="poolId">The pool identifier.</param>
        /// <returns></returns>
        private FitnessLog GetFitnessLog(int networkId, string poolId)
        {
            using (var db = new NetworkDataModelContainer())
            {
                var network = db.NetworkSet.First(n => n.NetworkId == networkId);
                var maxGeneration = network.TrainLogs
                                           .Where(tl => tl.MultipleGenPoolIdentifier.Equals(poolId))
                                           .Max(n => n.GenerationNr);

                List<string> patterns = network.TrainLogs.First()
                                               .PatternFitnessSet
                                               .Select(p => p.Pattern)
                                               .Distinct()
                                               .ToList();

                return new FitnessLog()
                {
                    FitnessTrend = network.TrainLogs
                        .Where(tl => tl.MultipleGenPoolIdentifier.Equals(poolId))
                        .OrderBy(tl => tl.GenerationNr)
                        .Select(tl => tl.Fitness)
                        .ToList<double>(),

                    FinalPatternFittness = network.TrainLogs
                        .First(tl => tl.MultipleGenPoolIdentifier.Equals(poolId)
                                        && tl.GenerationNr == maxGeneration)
                        .PatternFitnessSet.ToDictionary(i => i.Pattern, i => i.Fitness),


                    PatternTrends = patterns.ToDictionary(key => key, 
                        value => db.PatternFitnessSet
                                .Where(p => p.TrainLog.Network.NetworkId == networkId 
                                    && p.TrainLog.MultipleGenPoolIdentifier.Equals(poolId)
                                    && p.Pattern.Equals(value))
                                .OrderBy(i => i.TrainLog.GenerationNr)
                                .Select(i => i.Fitness)
                                .ToList<double>())
                };
            }
        }

        /// <summary>
        /// Creates the network.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <returns></returns>
        public bool CreateNetwork(string name, IEnumerable<TrainingImage> individualTrainingsData)
        {
            bool status = true;
            DataManager<double[,]> serializer = new DataManager<double[,]>();
            int networkId = -1; 

            using (var db = new NetworkDataModelContainer())
            {
                var newNetwork = db.NetworkSet.Add(new Network()
                {
                    NetworkName = name,
                    TrainingImages = new List<TrainingImageData>(),
                    Calculated = CalculationType.NotStarted
                });

                networkId = newNetwork.NetworkId;

                var newTrainigData = individualTrainingsData.Select(
                data => new TrainingImageData()
                {
                    Pattern = data.Pattern,
                    ImageData = serializer.TransformToBinary(data.TransformTo2DArrayFromImageData()),
                    Network = newNetwork
                });

                db.TrainingImageDataSet.AddRange(newTrainigData);

                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
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

        public bool CreateNetworkWithTrainingDataCopy(string name, IEnumerable<TrainingImage> individualTrainingsData, int copyTraindataFromNetworkId)
        {
            throw new NotImplementedException();
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
                var delNetwork = db.NetworkSet.First(n => n.NetworkId == networkId);

                db.PatternFitnessSet.RemoveRange(db.PatternFitnessSet.Where(pfs => pfs.TrainLog.Network.NetworkId == networkId));
                db.TrainLogSet.RemoveRange(delNetwork.TrainLogs);
                db.NetworkSet.Remove(delNetwork);

                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
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
            NetworkDataManager dataManager = new NetworkDataManager();
            PatternRecognitionNetwork network;
            StringBuilder numberBuilder = new StringBuilder();
            NumberRecognitionResult result = new NumberRecognitionResult
            {
                Items = new List<Contract.Data.NumberRecognitionResultItem>()
            };

            using (var db = new NetworkDataModelContainer())
            {
                var networkFromDb = db.NetworkSet.First(n => n.NetworkId == networkId);

                network = dataManager.TransformFromBinary(networkFromDb.NetworkData);
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
    }
}
