using System;

namespace NumberRecognizer.Lib.Network
{
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