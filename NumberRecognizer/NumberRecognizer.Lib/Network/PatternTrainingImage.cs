//-----------------------------------------------------------------------
// <copyright file="PatternTrainingImage.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>PatternTrainingImage - Neuronal Network.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;

    /// <summary>
    /// PatternTrainingImage.
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