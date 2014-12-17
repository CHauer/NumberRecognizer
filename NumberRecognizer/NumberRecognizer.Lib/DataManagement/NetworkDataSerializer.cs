//-----------------------------------------------------------------------
// <copyright file="NetworkDataSerializer.cs" company="FH Wr.Neustadt">
//     Copyright (c) Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>NetworkDataSerializer - Data Serializer for Binary Serializing Network Data.</summary>
//-----------------------------------------------------------------------

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
    /// The NetworkDataSerializer Class provides
    /// functionality to Save/Load a Network to/from file
    /// and Serialize a network to/from a byte array of data.
    /// </summary>
    public class NetworkDataSerializer : DataSerializer<PatternRecognitionNetwork>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDataSerializer"/> class.
        /// </summary>
        public NetworkDataSerializer()
        {
        }

    }
}
