using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    public class TrainingImage : RecognitionImage
    {
        public string Pattern { get; set; }

    }
}
