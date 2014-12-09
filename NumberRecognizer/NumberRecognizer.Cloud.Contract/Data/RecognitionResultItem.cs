using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    public class RecognitionResultItem
    {
        [DataMember]
        public int NumericCharacter { get; set; }

        [DataMember]
        public Dictionary<char, decimal> Probabilities { get; set; }
    }
}
