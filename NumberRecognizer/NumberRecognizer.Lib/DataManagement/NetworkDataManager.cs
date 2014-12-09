using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.DataManagement
{
    /// <summary>
    /// The NetworkDataManager Class provides
    /// functionalty to Save/Load a Network to/from file
    /// and Serialize a network to/from a byte array of data.
    /// </summary>
    public class NetworkDataManager : DataManager<PatternRecognitionNetwork>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDataManager"/> class.
        /// </summary>
        public NetworkDataManager()
        {
        }

    }
}
