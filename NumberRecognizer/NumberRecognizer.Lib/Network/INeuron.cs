//-----------------------------------------------------------------------
// <copyright file="INeuron.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>INeuron Interface for Nerons.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;

    /// <summary>
    /// The INeuron Interface.
    /// </summary>
	public interface INeuron
	{
        /// <summary>
        /// Gets the activation value.
        /// </summary>
        /// <value>
        /// The activation value.
        /// </value>
		double ActivationValue { get; }
	}
}