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
        RecognitionResultItem RecognizeOneNumber(double[,] pixelValues);

        [OperationContract]
        RecognitionResult RecognizePhoneNumber(double[,] pixelValues);

        [OperationContract]
        RecognitionResultItem RecognizeOneNumberFromImage(Byte[] imageData);

        [OperationContract]
        RecognitionResult RecognizePhoneNumberFromImage(Byte[] imageData);

    }
}
