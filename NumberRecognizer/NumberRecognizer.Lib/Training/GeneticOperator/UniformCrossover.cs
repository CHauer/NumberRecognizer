using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training.Contract;

namespace NumberRecognizer.Lib.Training.GeneticOperator
{
    public class UniformCrossover : ICrossover 
    {
        /// <summary>
        /// Executes the one point crossover.
        /// </summary>
        /// <param name="networks">The networks.</param>
        /// <returns></returns>
        public IEnumerable<PatternRecognitionNetwork> ExecuteCrossover(IList<PatternRecognitionNetwork> networks)
        {
            ThreadSafeRandom random = new ThreadSafeRandom();
            List<PatternRecognitionNetwork> resultNetworks = new List<PatternRecognitionNetwork>();

            //One time order - high performance costs
            List<PatternRecognitionNetwork> fixedOrderList = networks.OrderBy(individual => Guid.NewGuid()).ToList();            

            for (int i = 0; i < networks.Count; i++)
            {
                PatternRecognitionNetwork parentA = networks[i];
                PatternRecognitionNetwork parentB = fixedOrderList[i];
                PatternRecognitionNetwork childOne = new PatternRecognitionNetwork(parentA.InputWidth, parentA.InputHeight, parentA.Patterns);
                PatternRecognitionNetwork childTwo = new PatternRecognitionNetwork(parentA.InputWidth, parentA.InputHeight, parentA.Patterns);

                int genomeCount = parentA.Genomes.Count;

                for (int j = 0; j < genomeCount - 1; j++)
                {
                    if (random.NextDouble() >= 0.5)
                    {
                        childOne.Genomes[j].Weight = parentB.Genomes[j].Weight;
                        childTwo.Genomes[j].Weight = parentA.Genomes[j].Weight;
                    }
                    else
                    {
                        childOne.Genomes[j].Weight = parentA.Genomes[j].Weight;
                        childTwo.Genomes[j].Weight = parentB.Genomes[j].Weight;
                    }
                }

                resultNetworks.Add(childOne);
                resultNetworks.Add(childTwo);
            }

            return resultNetworks;
        }

    }
}
