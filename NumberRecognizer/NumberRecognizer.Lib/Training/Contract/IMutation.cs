//-----------------------------------------------------------------------
// <copyright file="IMutation.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>IMutation - Interface - Neuronal Network.</summary>
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
    /// The Mutation Genetic Operator Interface for the genetic algorithm.
    /// </summary>
    public interface IMutation
    {
        /// <summary>
        /// Gets or sets the minimum network fitness for he usage of this mutation operator.
        /// </summary>
        /// <value>
        /// The minimum network fitness.
        /// </value>
        double MinNetworkFitness { get; set; }

        /// <summary>
        /// Executes the mutation.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>Network object.</returns>
        PatternRecognitionNetwork ExecuteMutation(PatternRecognitionNetwork network);
    }
}
