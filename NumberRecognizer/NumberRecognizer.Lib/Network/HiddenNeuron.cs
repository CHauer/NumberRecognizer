//-----------------------------------------------------------------------
// <copyright file="HiddenNeuron.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>HiddenNeuron Network Neuron.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// HiddenNeuron Network Neuron.
    /// </summary>
    [Serializable]
    public class HiddenNeuron : INeuron, ICacheable
    {
        /// <summary>
        /// The cached activation value
        /// </summary>
        [NonSerialized]
        private double? cachedActivationValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiddenNeuron"/> class.
        /// </summary>
        public HiddenNeuron()
        {
            this.InputLayer = new List<WeightedLink>();
        }

        /// <summary>
        /// Gets the activation value.
        /// </summary>
        /// <value>
        /// The activation value.
        /// </value>
        public double ActivationValue
        {
            get
            {
                if (!this.cachedActivationValue.HasValue)
                {
                    ////double sum = InputLayer.Sum(x => x.Neuron.ActivationValue * x.Weight);

                    ////performance better than LINQ Sum
                    double sum = 0.0;

                    foreach (WeightedLink link in InputLayer)
                    {
                        sum += link.Neuron.ActivationValue * link.Weight;
                    }

                    ////Sigmoid
                    ////return ((1 / (1 + Math.Pow(Math.E, sum * -1))) * 2) - 1;
                    this.cachedActivationValue = ((1 / (1 + Math.Exp(sum * -1))) * 2) - 1;
                }

                return this.cachedActivationValue.Value;
            }
        }

        /// <summary>
        /// Gets or sets the input layer.
        /// </summary>
        /// <value>
        /// The input layer.
        /// </value>
        public List<WeightedLink> InputLayer { get; private set; }

        /// <summary>
        /// Resets the cached activation value.
        /// </summary>
        public void ResetCachedValue()
        {
            this.cachedActivationValue = null;
        }
    }
}
