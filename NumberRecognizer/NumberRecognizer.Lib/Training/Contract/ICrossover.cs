//-----------------------------------------------------------------------
// <copyright file="ICrossover.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>ICrossover - Interface - Neuronal Network.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Training.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumberRecognizer.Lib.Network;

    /// <summary>
    /// The Crossover Genetic Operator functionality interface for the genetic algorithm.
    /// </summary>
    public interface ICrossover
    {
        /// <summary>
        /// Executes the crossover.
        /// </summary>
        /// <param name="networks">The networks.</param>
        /// <returns>Network object.</returns>
        IEnumerable<PatternRecognitionNetwork> ExecuteCrossover(IList<PatternRecognitionNetwork> networks);
    }
}
