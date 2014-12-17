using System;

namespace NumberRecognizer.Lib.Network
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public class WeightedLink
	{
        /// <summary>
        /// Gets or sets the neuron.
        /// </summary>
        /// <value>
        /// The neuron.
        /// </value>
		public INeuron Neuron { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
		public double Weight { get; set; }
    }
}