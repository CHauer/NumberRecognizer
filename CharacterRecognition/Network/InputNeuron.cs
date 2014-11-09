namespace NumberRecognizer.Lib.Network
{
	public class InputNeuron : INeuron
	{
		public double ActivationValue
		{
			get { return Value; }
		}

		public double Value { get; set; }
	}
}