using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NumberRecognizer.Cloud.Contract;
using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.Cloud.Service
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Klassennamen "NumberRecognizerService" sowohl im Code als auch in der SVC- und der Konfigurationsdatei ändern.
    // HINWEIS: Wählen Sie zum Starten des WCF-Testclients zum Testen dieses Diensts NumberRecognizerService.svc oder NumberRecognizerService.svc.cs im Projektmappen-Explorer aus, und starten Sie das Debuggen.
    public class NumberRecognizerService :  INumberRecognizerService
    {

        public IList<NetworkInfo> GetNetworks()
        {
            throw new NotImplementedException();
        }

        public bool CreateNetwork(string name, IEnumerable<TrainingImage> individualTrainingsData)
        {
            throw new NotImplementedException();
        }

        public bool CreateNetwork(string name, IEnumerable<TrainingImage> individualTrainingsData, int copyTraindataFromNetworkId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteNetwork(int networkId)
        {
            throw new NotImplementedException();
        }

        public RecognitionResult RecognizePhoneNumber(int networkId, IList<RecognitionImage> imageData)
        {
            throw new NotImplementedException();
        }
    }
}
