//-----------------------------------------------------------------------
// <copyright file="OnePointCrossover.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>OnePointCrossover - Genetic Parameter.</summary>
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
    /// OnePointCrossover - Genetic Parameter.
    /// </summary>
    public class OnePointCrossover : ICrossover 
    {
        /// <summary>
        /// Executes the one point crossover.
        /// </summary>
        /// <param name="networks">The networks.</param>
        /// <returns>
        /// Network object.
        /// </returns>
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
                int crossoverPoint = random.Next(genomeCount - 1);

                for (int j = 0; j < genomeCount - 1; j++)
                {
                    if (j < crossoverPoint)
                    {
                        childOne.Genomes[j].Weight = parentA.Genomes[j].Weight;
                        childTwo.Genomes[j].Weight = parentB.Genomes[j].Weight;
                    }
                    else
                    {
                        childOne.Genomes[j].Weight = parentB.Genomes[j].Weight;
                        childTwo.Genomes[j].Weight = parentA.Genomes[j].Weight;
                    }
                }

                resultNetworks.Add(childOne);
                resultNetworks.Add(childTwo);
            }

            return resultNetworks;
        }

    }
}
