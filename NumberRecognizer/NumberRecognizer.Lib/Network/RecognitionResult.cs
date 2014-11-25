using System;

namespace NumberRecognizer.Lib.Network
{
    [Serializable] 
	public class RecognitionResult
	{
		public double Propability { get; set; }

		public string RecognizedCharacter { get; set; }
	}
}