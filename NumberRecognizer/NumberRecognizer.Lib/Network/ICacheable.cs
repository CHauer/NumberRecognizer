//-----------------------------------------------------------------------
// <copyright file="ICacheable.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>ICacheable Interface for Nerons.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface provides functionality to
    /// reset the cached activation value.
    /// Marks the Neuron 
    /// </summary>
    public interface ICacheable
    {

        /// <summary>
        /// Resets the cached activation value.
        /// </summary>
        void ResetCachedValue();

    }
}
