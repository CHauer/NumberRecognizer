using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training.Contract;

namespace NumberRecognizer.Lib.Training.GeneticOperator
{
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
        /// TODO Uniform - only one Genone can be changed
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns></returns>
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
