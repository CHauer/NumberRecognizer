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
    public class NetworkDataManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDataManager"/> class.
        /// </summary>
        public NetworkDataManager()
        {
            
        }

        /// <summary>
        /// Loads the network from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public PatternRecognitionNetwork LoadNetworkFromFile(string path)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                using (Stream fileStream = File.OpenRead(path))
                {
                    return (PatternRecognitionNetwork)serializationBinFormatter.Deserialize(fileStream);
                }
            }
            catch (SerializationException seEx) { ;}
            catch (IOException ioex) { ;}
            catch (Exception ex){;}

            return null;
        }

        /// <summary>
        /// Saves the network to file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="network">The network.</param>
        /// <returns></returns>
        public bool SaveNetworkToFile(string path, PatternRecognitionNetwork network)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                using (Stream fileStream = File.Open(path, FileMode.Create))
                {
                    serializationBinFormatter.Serialize(fileStream, network);

                    fileStream.Flush();
                }
            }
            catch (SerializationException seEx)
            {
                return false;
            }
            catch (IOException ioEx)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Transforms the network to binary.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns></returns>
        public Byte[] TransformNetworkToBinary(PatternRecognitionNetwork network)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    serializationBinFormatter.Serialize(stream, network);

                    stream.Flush();
                    return stream.ToArray();
                }
            }
            catch (SerializationException seEx) { ;}
            catch (Exception ex) { ;}

            return null;
        }

        /// <summary>
        /// Transforms the binary to network.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public PatternRecognitionNetwork TransformBinaryToNetwork(Byte[] data)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                MemoryStream stream = new MemoryStream(data);

                return (PatternRecognitionNetwork)serializationBinFormatter.Deserialize(stream);
            }
            catch (SerializationException seEx) { ;}
            catch (Exception ex) { ;}

            return null;
        }
    }
}
