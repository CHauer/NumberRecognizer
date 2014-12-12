using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    public class FitnessLog
    {
        /// <summary>
        /// Gets or sets the fitness trend from generation 0 to n.
        /// </summary>
        /// <value>
        /// The fitness trend in double values.
        /// </value>
        [DataMember]
        public IList<double> FitnessTrend { get; set; }

        /// <summary>
        /// Gets or sets the pattern trends.
        /// The Key of the dictionary represents the pattern (eg. 0-9)
        /// The list of double values the generations from 0 - n.
        /// </summary>
        /// <value>
        /// The pattern trends.
        /// </value>
        [DataMember]
        public Dictionary<string, List<double>> PatternTrends { get; set; }

        /// <summary>
        /// Gets or sets the final pattern fittness.
        /// The key represents the pattern (eg. 0-9)
        /// The value represents the fittness for the final generation of the network.
        /// </summary>
        /// <value>
        /// The final pattern fittness.
        /// </value>
        [DataMember]
        public Dictionary<string, double>  FinalPatternFittness { get; set; }

    }
}
