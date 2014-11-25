using System;

namespace NumberRecognizer.Lib.Network
{
    [Serializable]
	public class WeightedLink
	{
		public INeuron Neuron { get; set; }

		public double Weight { get; set; }
    }
}