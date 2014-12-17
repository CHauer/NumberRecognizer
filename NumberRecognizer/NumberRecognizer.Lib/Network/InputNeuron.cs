//-----------------------------------------------------------------------
// <copyright file="InputNeuron.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>InputNeuron Classes.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;

    /// <summary>
    /// The Input Neuron for a neuronal network.
    /// </summary>
    [Serializable]
	public class InputNeuron : INeuron
	{
        /// <summary>
        /// Gets the activation value.
        /// </summary>
        /// <value>
        /// The activation value.
        /// </value>
		public double ActivationValue
		{
			get { return Value; }
		}

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
		public double Value { get; set; }
	}
}