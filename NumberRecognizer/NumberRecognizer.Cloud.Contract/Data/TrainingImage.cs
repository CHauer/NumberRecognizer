//-----------------------------------------------------------------------
// <copyright file="TrainingImage.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>TrainingImage Data Contract.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    /// <summary>
    /// Training Data Contract.
    /// </summary>
    [DataContract]
    [KnownType(typeof(RecognitionImage))]
    public class TrainingImage : RecognitionImage
    {
        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        [DataMember]
        public string Pattern { get; set; }

    }
}
