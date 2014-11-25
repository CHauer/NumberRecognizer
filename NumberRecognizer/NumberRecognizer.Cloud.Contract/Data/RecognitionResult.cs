using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    public class RecognitionResult
    {
        [DataMember]
        public String Number { get; set; }

        [DataMember]
        public List<RecognitionResultItem> Items { get; set; }
    }
}
