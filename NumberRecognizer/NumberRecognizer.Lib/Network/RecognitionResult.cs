using System;

namespace NumberRecognizer.Lib.Network
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable] 
	public class RecognitionResult
	{
        /// <summary>
        /// Gets or sets the propability.
        /// </summary>
        /// <value>
        /// The propability.
        /// </value>
		public double Propability { get; set; }

        /// <summary>
        /// Gets or sets the recognized character.
        /// </summary>
        /// <value>
        /// The recognized character.
        /// </value>
		public string RecognizedCharacter { get; set; }
	}
}