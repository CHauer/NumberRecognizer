using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    public class RecognitionImage
    {
        public double[] ImageData { get; set; }

        public int Height { get; set; }

        public int Width{ get; set; }

    }
}
