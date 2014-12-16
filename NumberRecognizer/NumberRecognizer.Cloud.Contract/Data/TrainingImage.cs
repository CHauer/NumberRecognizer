using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    /// <summary>
    /// 
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
