using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    public class NumberRecognitionResult
    {
        [DataMember]
        public String Number { get; set; }

        [DataMember]
        public List<NumberRecognitionResultItem> Items { get; set; }
    }
}
