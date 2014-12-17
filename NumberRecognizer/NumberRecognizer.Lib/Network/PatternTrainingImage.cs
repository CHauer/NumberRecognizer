using System;

namespace NumberRecognizer.Lib.Network
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public class PatternTrainingImage
	{
        /// <summary>
        /// Gets or sets the pixel values.
        /// </summary>
        /// <value>
        /// The pixel values.
        /// </value>
		public double[,] PixelValues { get; set; }

        /// <summary>
        /// Gets or sets the representing information.
        /// </summary>
        /// <value>
        /// The representing information.
        /// </value>
		public string RepresentingInformation { get; set; }
	}
}