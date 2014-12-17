using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// The DataSerializer Class provides
    /// functionalty to Save/Load a instance of T to/from file
    /// and Serialize a instance to/from a byte array of data.
    /// </summary>
    public class DataSerializer<T> where T : class
    {

        /// <summary>
        /// Loads the binary data from file into object T instance.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public T LoadFromFile(string path)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                using (Stream fileStream = File.OpenRead(path))
                {
                    return (T)serializationBinFormatter.Deserialize(fileStream);
                }
            }
            catch (SerializationException seEx)
            {
                Debug.WriteLine(seEx.Message);
            }
            catch (IOException ioex)
            {
                Debug.WriteLine(ioex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Saves the data in binary format to file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="transformObj">The transform object.</param>
        /// <returns></returns>
        public bool SaveToFile(string path, T transformObj)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                using (Stream fileStream = File.Open(path, FileMode.Create))
                {
                    serializationBinFormatter.Serialize(fileStream, transformObj);

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
        /// Transforms the instance of T to binary data.
        /// </summary>
        /// <param name="transformObj">The transform object.</param>
        /// <returns></returns>
        public Byte[] TransformToBinary(T transformObj)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    serializationBinFormatter.Serialize(stream, transformObj);

                    stream.Flush();
                    return stream.ToArray();
                }
            }
            catch (SerializationException seEx)
            {
                Debug.WriteLine(seEx.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Transforms the binary to instance of T.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public T TransformFromBinary(Byte[] data)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                MemoryStream stream = new MemoryStream(data);

                return (T)serializationBinFormatter.Deserialize(stream);
            }
            catch (SerializationException seEx) { ;}
            catch (Exception ex) { ;}

            return null;
        }
    }
}
