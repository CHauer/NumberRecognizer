﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
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
        RecognitionResult RecognizePhoneNumber(int networkId, IList<RecognitionImage> imageData);

    }
}
