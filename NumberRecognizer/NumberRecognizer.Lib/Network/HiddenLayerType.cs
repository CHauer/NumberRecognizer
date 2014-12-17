//-----------------------------------------------------------------------
// <copyright file="HiddenLayerType.cs" company="FH Wr.Neustadt">
//     Copyright (c) Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>HiddenLayerType of neuronal network.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Hidden Layer Type.
    /// </summary>
    [Flags]
    public enum HiddenLayerType
    {
        /// <summary>
        /// The lining
        /// </summary>
        Lining = 1,
        /// <summary>
        /// The boxing
        /// </summary>
        Boxing = 2,
        /// <summary>
        /// The lining
        /// </summary>
        OverlayedLining = 4,
        /// <summary>
        /// The boxing
        /// </summary>
        OverlayedBoxing = 8,
        /// <summary>
        /// The striping
        /// </summary>
        Striping = 16
    }
}
