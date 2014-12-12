using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    [KnownType(typeof(RecognitionImage))]
    public class TrainingImage : RecognitionImage
    {
        [DataMember]
        public string Pattern { get; set; }

    }
}
