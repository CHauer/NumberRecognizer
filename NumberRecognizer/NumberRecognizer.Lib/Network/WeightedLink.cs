//-----------------------------------------------------------------------
// <copyright file="WeightedLink.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>WeightedLink - Neuronal Network.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;

    /// <summary>
    /// WeightedLink Class.
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