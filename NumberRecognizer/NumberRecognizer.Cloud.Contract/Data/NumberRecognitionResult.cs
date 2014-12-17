//-----------------------------------------------------------------------
// <copyright file="NumberRecognitionResult.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>NumberRecognitionResult Data Contract.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Cloud.Contract.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// NumberRecognitionResult Data Contract.
    /// </summary>
    [DataContract]
    public class NumberRecognitionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberRecognitionResult"/> class.
        /// </summary>
        public NumberRecognitionResult()
        {
            Items = new List<NumberRecognitionResultItem>();
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        [DataMember]
        public String Number { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [DataMember]
        public List<NumberRecognitionResultItem> Items { get; set; }
    }
}
