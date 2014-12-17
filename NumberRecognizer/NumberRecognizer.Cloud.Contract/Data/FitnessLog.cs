//-----------------------------------------------------------------------
// <copyright file="FitnessLog.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>FitnessLog DataContract.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Cloud.Contract.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// FitnessLog Data Contract.
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

    }
}
