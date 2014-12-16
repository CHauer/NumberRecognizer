using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Cloud.Contract.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class FitnessLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FitnessLog"/> class.
        /// </summary>
        public FitnessLog()
        {
            FitnessTrend = new List<double>();
            //PatternTrends = new Dictionary<string, List<double>>();
        }

        /// <summary>
        /// Gets or sets the fitness trend from generation 0 to n.
        /// </summary>
        /// <value>
        /// The fitness trend in double values.
        /// </value>
        [DataMember]
        public IList<double> FitnessTrend { get; set; }

        ///// <summary>
        ///// Gets or sets the pattern trends.
        ///// The Key of the dictionary represents the pattern (eg. 0-9)
        ///// The list of double values the generations from 0 - n.
        ///// </summary>
        ///// <value>
        ///// The pattern trends.
        ///// </value>
        //[DataMember]
        //public Dictionary<string, List<double>> PatternTrends { get; set; }

    }
}
