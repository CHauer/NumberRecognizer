//-----------------------------------------------------------------------
// <copyright file="DataSerializer.cs" company="FH Wr.Neustadt">
//     Copyright (c) Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>Data Serializer for Binary Serializing Data.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.Lib.DataManagement
{
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

    /// <summary>
    /// The DataSerializer Class provides
    /// functionality to Save/Load a instance of T to/from file
    /// and Serialize a instance to/from a byte array of data.
    /// </summary>
    public class DataSerializer<T> where T : class
    {

        /// <summary>
        /// Loads the binary data from file into object T instance.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Object Type of T</returns>
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
        /// <returns>Status of Save To File Operation.</returns>
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
                Debug.WriteLine(seEx.Message);
                return false;
            }
            catch (IOException ioEx)
            {
                Debug.WriteLine(ioEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Transforms the instance of T to binary data.
        /// </summary>
        /// <param name="transformObj">The transform object.</param>
        /// <returns>Return Byte Array of Object.</returns>
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
        /// <returns>Returns Object from Type of T.</returns>
        public T TransformFromBinary(Byte[] data)
        {
            BinaryFormatter serializationBinFormatter = new BinaryFormatter();

            try
            {
                MemoryStream stream = new MemoryStream(data);

                return (T)serializationBinFormatter.Deserialize(stream);
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
    }
}
