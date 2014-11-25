using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.DataManagement
{
    public class NetworkDataManager
    {
        public NetworkDataManager()
        {
        }

        public PatternRecognitionNetwork LoadNetworkFromFile(string path)
        {
            throw new NotImplementedException();
        }

        public bool SaveNetworkToFile(string path, PatternRecognitionNetwork network)
        {
            throw new NotImplementedException();
        }

        public Byte[] TransformNetworkToBinary(PatternRecognitionNetwork network)
        {
            throw new NotImplementedException();
        }

        public PatternRecognitionNetwork TransformBinaryToNetwork(Byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
