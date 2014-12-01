using System;
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

        [OperationContract]
        IList<NetworkInfo> GetNetworks();

        [OperationContract]
        Dictionary<int, string> GetBaseTraningSets();

        [OperationContract]
        bool CreateNetwork(string name, int baseTrainingsSetId, Dictionary<string, IEnumerable<double[,]>> individualTrainingsData);

        [OperationContract]
        RecognitionResultItem RecognizeOneNumber(int networkId, double[,] pixelValues);

        [OperationContract]
        RecognitionResult RecognizePhoneNumber(int networkId, double[,] pixelValues);

        [OperationContract]
        RecognitionResultItem RecognizeOneNumberFromImage(int networkId, Byte[] imageData);

        [OperationContract]
        RecognitionResult RecognizePhoneNumberFromImage(int networkId, Byte[] imageData);

    }
}
