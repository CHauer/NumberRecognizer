using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training.Contract;

namespace NumberRecognizer.Lib.Training.GeneticOperator
{
    public class GaussMutation : IMutation
    {
        public GaussMutation()
        {
            MinMutationRange = -0.5;
            MaxMutationRange =  0.5;
            GaussStandardDeviation = 1;
            MutationRatio = 0.95;
        }

        public double MinMutationRange { get; set; }

        public double MaxMutationRange { get; set; }

        public double GaussStandardDeviation{ get; set; }

        public double MutationRatio{ get; set; }

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
