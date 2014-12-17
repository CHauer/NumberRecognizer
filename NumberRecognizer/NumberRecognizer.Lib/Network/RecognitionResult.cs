//-----------------------------------------------------------------------
// <copyright file="RecognitionResult.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>RecognitionResult - Neuronal Network.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;

    /// <summary>
    /// Recognition Result.
    /// </summary>
    [Serializable]
    public class RecognitionResult
    {
        /// <summary>
        /// Gets or sets the probability.
        /// </summary>
        /// <value>
        /// The probability.
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