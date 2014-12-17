//-----------------------------------------------------------------------
// <copyright file="INumberRecognizerService.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>INumberRecognizerService Operation Service Contract.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Cloud.Contract
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.ServiceModel;
    using NumberRecognizer.Cloud.Contract.Data;

    /// <summary>
    /// The NumberRecognizer Service interface.
    /// Describes the communication between service and clients.
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
        /// Gets the network detail.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns>A current available network.</returns>
        [OperationContract]
        NetworkInfo GetNetworkDetail(int networkId);

        /// <summary>
        /// Creates the network.
        /// </summary>
        /// <param name="networkName">Name of the network.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <returns>Status Flag.</returns>
        [OperationContract]
        bool CreateNetwork(string networkName, IEnumerable<TrainingImage> individualTrainingsData);

        /// <summary>
        /// Creates the network and copies the training data from a previous network.
        /// </summary>
        /// <param name="networkName">Name of the network.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <param name="copyTraindataFromNetworkId">The copy training data from network identifier.</param>
        /// <returns>Status</returns>
        [OperationContract]
        bool CreateNetworkWithTrainingDataCopy(string networkName, IEnumerable<TrainingImage> individualTrainingsData, int copyTraindataFromNetworkId);

        /// <summary>
        /// Deletes the network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns>Status</returns>
        [OperationContract]
        bool DeleteNetwork(int networkId);

        /// <summary>
        /// Res the train network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns>Status</returns>
        [OperationContract]
        bool ReTrainNetwork(int networkId);

        /// <summary>
        /// Recognizes the phone number from image.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="imageData">The image data.</param>
        /// <returns>Recognition Result.</returns>
        [OperationContract]
        NumberRecognitionResult RecognizePhoneNumber(int networkId, IList<RecognitionImage> imageData);

    }
}
