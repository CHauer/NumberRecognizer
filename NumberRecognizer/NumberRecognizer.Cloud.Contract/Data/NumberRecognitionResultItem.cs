using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    public class NumberRecognitionResultItem
    {
        [DataMember]
        public int NumericCharacter { get; set; }

        [DataMember]
        public Dictionary<char, double> Probabilities { get; set; }
    }
}
