//-----------------------------------------------------------------------
// <copyright file="UniformMutation.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>UniformMutation - Genetic Parameter.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Training.GeneticOperator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumberRecognizer.Lib.Network;
    using NumberRecognizer.Lib.Training.Contract;

    /// <summary>
    /// UniformMutation - Genetic Parameter.
    /// </summary>
    public class UniformMutation : IMutation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniformMutation"/> class.
        /// </summary>
        public UniformMutation()
        {
            MutationRatio = 0.95;
        }

        /// <summary>
        /// Gets or sets the mutation ratio.
        /// </summary>
        /// <value>
        /// The mutation ratio.
        /// </value>
        public double MutationRatio { get; set; }

        /// <summary>
        /// Gets or sets the minimum network fitness for he usage of this mutation operator.
        /// </summary>
        /// <value>
        /// The minimum network fitness.
        /// </value>
        public double MinNetworkFitness { get; set; }

        /// <summary>
        /// Executes the mutation.
        /// Simple random mutation - Uniform Mutation
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>The modified network.</returns>
        public PatternRecognitionNetwork ExecuteMutation(PatternRecognitionNetwork network)
        {
            ThreadSafeRandom random = new ThreadSafeRandom();

            // Simple random mutation
            network.Genomes.ForEach(genome =>
            {
                if (random.NextDouble() > MutationRatio)
                {
                    genome.Weight += (random.NextDouble() * 2) - 1;
                }
            });

            return network;
        }

    }
}
