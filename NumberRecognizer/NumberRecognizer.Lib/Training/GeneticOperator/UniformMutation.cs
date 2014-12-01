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
                if (random.NextDouble() > 0.95)
                {
                    genome.Weight += (random.NextDouble() * 2) - 1;
                }
            });

            return network;
        }
    }
}
