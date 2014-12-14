using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Cloud.Contract.Data
{
    /// <summary>
    /// The network info class represents the information of one available network.
    /// </summary>
    [DataContract]
    public class NetworkInfo
    {
        /// <summary>
        /// Gets or sets the network identifier.
        /// </summary>
        /// <value>
        /// The network identifier.
        /// </value>
        [DataMember]
        public int NetworkId { get; set; }

        /// <summary>
        /// Gets or sets the name of the network.
        /// </summary>
        /// <value>
        /// The name of the network.
        /// </value>
        [DataMember]
        public string NetworkName{ get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the network is 
        /// calculated/trained and can recognize numbers.
        /// </summary>
        /// <value>
        ///   <c>true</c> if calculated/trained; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Calculated { get; set; }

        /// <summary>
        /// Gets or sets the network fitness.
        /// </summary>
        /// <value>
        /// The network fitness.
        /// </value>
        [DataMember]
        public double NetworkFitness { get; set; }

        /// <summary>
        /// Gets or sets the final pool fitness log.
        /// </summary>
        /// <value>
        /// The final pool fitness log.
        /// </value>
        [DataMember]
        public FitnessLog FinalPoolFitnessLog { get; set; }

        /// <summary>
        /// Gets or sets the multiple pool fitness log.
        /// </summary>
        /// <value>
        /// The multiple pool fitness log.
        /// </value>
        [DataMember]
        public Dictionary<string, FitnessLog> MultiplePoolFitnessLog { get; set; }

        /// <summary>
        /// Gets or sets the calculation start.
        /// </summary>
        /// <value>
        /// The calculation start.
        /// </value>
        [DataMember]
        public DateTime? CalculationStart { get; set; }

        /// <summary>
        /// Gets or sets the calculation end.
        /// </summary>
        /// <value>
        /// The calculation end.
        /// </value>
        [DataMember]
        public DateTime? CalculationEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [multiple gen pool].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multiple gen pool]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool MultipleGenPool{ get; set; }
    }
}
