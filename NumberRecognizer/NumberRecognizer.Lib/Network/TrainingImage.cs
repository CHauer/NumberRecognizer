using System;

namespace NumberRecognizer.Lib.Network
{
    [Serializable]
	public class TrainingImage
	{
		public double[,] PixelValues { get; set; }

		public string RepresentingInformation { get; set; }
	}
}