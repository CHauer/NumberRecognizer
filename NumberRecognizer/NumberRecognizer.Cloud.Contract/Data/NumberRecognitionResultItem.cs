//-----------------------------------------------------------------------
// <copyright file="NumberRecognitionResultItem.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>NumberRecognitionResultItem Data Contract.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Cloud.Contract.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// NumberRecognitionResultItem Data Contract.
    /// </summary>
    [DataContract]
    public class NumberRecognitionResultItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberRecognitionResultItem"/> class.
        /// </summary>
        public NumberRecognitionResultItem()
        {
            Probabilities = new Dictionary<char, double>();
        }

        /// <summary>
        /// Gets or sets the numeric character.
        /// </summary>
        /// <value>
        /// The numeric character.
        /// </value>
        [DataMember]
        public int NumericCharacter { get; set; }

        /// <summary>
        /// Gets or sets the probabilities.
        /// </summary>
        /// <value>
        /// The probabilities.
        /// </value>
        [DataMember]
        public Dictionary<char, double> Probabilities { get; set; }
    }
}
