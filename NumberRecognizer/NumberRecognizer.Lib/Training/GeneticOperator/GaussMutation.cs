//-----------------------------------------------------------------------
// <copyright file="GaussMutation.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>GaussMutation - Genetic Parameter.</summary>
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
    /// GaussMutation - Genetic Parameter.
    /// </summary>
    public class GaussMutation : IMutation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GaussMutation"/> class.
        /// </summary>
        public GaussMutation()
        {
            MinMutationRange = -0.5;
            MaxMutationRange = 0.5;
            GaussStandardDeviation = 1;
            MutationRatio = 0.95;
            MinNetworkFitness = 0.90;
        }

        /// <summary>
        /// Gets or sets the minimum mutation range.
        /// </summary>
        /// <value>
        /// The minimum mutation range.
        /// </value>
        public double MinMutationRange { get; set; }

        /// <summary>
        /// Gets or sets the maximum mutation range.
        /// </summary>
        /// <value>
        /// The maximum mutation range.
        /// </value>
        public double MaxMutationRange { get; set; }

        /// <summary>
        /// Gets or sets the gauss standard deviation.
        /// </summary>
        /// <value>
        /// The gauss standard deviation.
        /// </value>
        public double GaussStandardDeviation { get; set; }

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
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>The modified network.</returns>
        public PatternRecognitionNetwork ExecuteMutation(PatternRecognitionNetwork network)
        {
            ThreadSafeRandom random = new ThreadSafeRandom();

            // gauss mutation
            network.Genomes.ForEach(genome =>
            {
                if (random.NextDouble() > MutationRatio) // 0.95
                {
                    double randomValue = random.NextGaussian(0, GaussStandardDeviation); //1

                    if (randomValue > MinMutationRange && randomValue < MaxMutationRange) //-0.5 / +0.5
                    {
                        genome.Weight += randomValue;
                    }
                }
            });

            return network;
        }

    }
}
