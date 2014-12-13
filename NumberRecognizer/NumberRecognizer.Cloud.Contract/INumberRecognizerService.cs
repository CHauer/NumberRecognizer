using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ServiceModel;
using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.Cloud.Contract
{
    /// <summary>
    /// The NumberRecognizer Service interface.
    /// Describes the communcation between service and clients.
    /// </summary>
    [ServiceContract]
    public interface INumberRecognizerService
    {
        /// <summary>
        /// Gets the networks.
        /// </summary>
        /// <returns>A list of current available networks.</returns>
        [OperationContract]
        IList<NetworkInfo> GetNetworks();

        /// <summary>
        /// Creates the network.
        /// </summary>
        /// <param name="networkName">Name of the network.</param>
        /// <param name="username">The username.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <returns></returns>
        [OperationContract]
        bool CreateNetwork(string networkName, string username, IEnumerable<TrainingImage> individualTrainingsData);

        /// <summary>
        /// Creates the network and copies the training data from a previous network.
        /// </summary>
        /// <param name="networkName">Name of the network.</param>
        /// <param name="username">The username.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <param name="copyTraindataFromNetworkId">The copy traindata from network identifier.</param>
        /// <returns></returns>
        [OperationContract]
        bool CreateNetworkWithTrainingDataCopy(string networkName, string username, IEnumerable<TrainingImage> individualTrainingsData, int copyTraindataFromNetworkId);

        /// <summary>
        /// Deletes the network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns></returns>
        [OperationContract]
        bool DeleteNetwork(int networkId);

        /// <summary>
        /// Recognizes the phone number from image.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="imageData">The image data.</param>
        /// <returns></returns>
        [OperationContract]
        NumberRecognitionResult RecognizePhoneNumber(int networkId, IList<RecognitionImage> imageData);

    }
}
