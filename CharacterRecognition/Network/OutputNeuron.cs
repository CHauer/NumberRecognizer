namespace NumberRecognizer.Lib.Network
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class OutputNeuron : INeuron
	{
		public OutputNeuron()
		{
			InputLayer = new List<WeightedLink>();
		}

		public double ActivationValue
		{
			get
			{
				double sum = InputLayer.Sum(x => x.Neuron.ActivationValue * x.Weight);

				return ((1 / (1 + Math.Pow(Math.E, sum * -1))) * 2) - 1;
			}
		}

		public List<WeightedLink> InputLayer { get; set; }
	}
}