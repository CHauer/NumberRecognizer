using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.App.DataModel
{
    public class LocalTrainingImage
    {
        public TrainingImage ImageData { get; set; }

        public String LocalImagePath { get; set; }
    }
}
