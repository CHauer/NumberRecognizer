using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    public class NetworkInfo
    {
        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public string NetworkName{ get; set; }
        
        [DataMember]
        public bool Calculated { get; set; }

        [DataMember]
        public double NetworkFitness { get; set; }

        [DataMember]
        public FitnessLog FinalPoolFitnessLog { get; set; }

        [DataMember]
        public Dictionary<string, FitnessLog> MultiplePoolFitnessLog { get; set; }

        [DataMember]
        public DateTime CalculationStart { get; set; }

        [DataMember]
        public DateTime CalculationEnd { get; set; }

        [DataMember]
        public bool MultipleGenPool{ get; set; }
    }
}
