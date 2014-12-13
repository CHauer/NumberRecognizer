using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ServiceModel;
using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.Cloud.Contract
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Schnittstellennamen "INumberRecognizerService" sowohl im Code als auch in der Konfigurationsdatei ändern.
    [ServiceContract]
    public interface INumberRecognizerService
    {

        /// <summary>
        /// Gets the networks.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<NetworkInfo> GetNetworks();

        /// <summary>
        /// Creates the network.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <returns></returns>
        [OperationContract]
        bool CreateNetwork(string name, IEnumerable<TrainingImage> individualTrainingsData);

        /// <summary>
        /// Creates the network and copies the training data from a previous network.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="individualTrainingsData">The individual trainings data.</param>
        /// <param name="copyTraindataFromNetworkId">The copy traindata from network identifier.</param>
        /// <returns></returns>
        [OperationContract]
        bool CreateNetwork(string name, IEnumerable<TrainingImage> individualTrainingsData, int copyTraindataFromNetworkId);

        /// <summary>
        /// Deletes the network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns></returns>
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
