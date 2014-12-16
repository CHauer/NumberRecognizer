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
    public class NumberRecognitionResultItem
    {
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
