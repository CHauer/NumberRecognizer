//-----------------------------------------------------------------------
// <copyright file="ISelection.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>IMutation - Interface - Neuronal Network.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Training.Contract
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumberRecognizer.Lib.Network;

    /// <summary>
    /// The Interface for the Selection Genetic Operator for the genetic algorithm.
    /// </summary>
    public interface ISelection
    {

        /// <summary>
        /// Executes the selection operator.
        /// </summary>
        /// <param name="currentGeneration">The current generation.</param>
        /// <returns>Network object.</returns>
        IEnumerable<PatternRecognitionNetwork> ExecuteSelection(ConcurrentBag<PatternRecognitionNetwork> currentGeneration);
    }
}
