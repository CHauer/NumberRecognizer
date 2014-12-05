using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberRecognizer.Lib.Network
{

    [Serializable]
	public class OutputNeuron : INeuron, ICacheable
	{
        /// <summary>
        /// The cached activation value
        /// </summary>
        [NonSerialized]
        private double? cachedActivationValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputNeuron"/> class.
        /// </summary>
		public OutputNeuron()
		{
			InputLayer = new List<WeightedLink>();
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
                if (!cachedActivationValue.HasValue)
                {
                    //double sum = InputLayer.Sum(x => x.Neuron.ActivationValue * x.Weight);
                    double sum = 0.0;

                    foreach (WeightedLink link in InputLayer)
                    {
                        sum += link.Neuron.ActivationValue * link.Weight;
                    }

                    //Sigmoid
                    //return ((1 / (1 + Math.Pow(Math.E, sum * -1))) * 2) - 1;
                    cachedActivationValue = ((1 / (1 + Math.Exp(sum * -1))) * 2) - 1;
                }

                return cachedActivationValue.Value;
			}
		}

        /// <summary>
        /// Gets or sets the input layer.
        /// </summary>
        /// <value>
        /// The input layer.
        /// </value>
		public List<WeightedLink> InputLayer { get; set; }

        /// <summary>
        /// Resets the cached activation value.
        /// </summary>
        public void ResetCachedValue()
        {
            cachedActivationValue = null;
        }
	}
}