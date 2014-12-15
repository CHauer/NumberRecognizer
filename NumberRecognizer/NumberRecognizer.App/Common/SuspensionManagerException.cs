//-----------------------------------------------------------------------
// <copyright file="SuspensionManagerException.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Suspension Manager Exception.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.Common
{
    using System;

    /// <summary>
    /// Suspension Manager Exception.
    /// </summary>
    public class SuspensionManagerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionManagerException"/> class.
        /// </summary>
        public SuspensionManagerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionManagerException"/> class.
        /// </summary>
        /// <param name="e">The Exception.</param>
        public SuspensionManagerException(Exception e)
            : base("SuspensionManager failed", e)
        {
        }
    }
}
