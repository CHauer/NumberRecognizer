using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Cloud.Contract.Data
{
    public class NetworkInfo
    {
        public int NetworkId { get; set; }

        public int NetworkName{ get; set; }


        public double NetworkFitness { get; set; }

        public FitnessLog FitnessLog { get; set; }
    }
}
