using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.Training.Contract
{
    /// <summary>
    /// The Crossover Genetic Operator functionality interface for the genetic algorithm.
    /// </summary>
    public interface ICrossover
    {
        /// <summary>
        /// Executes the crossover.
        /// </summary>
        /// <param name="networks">The networks.</param>
        /// <returns></returns>
        IEnumerable<PatternRecognitionNetwork> ExecuteCrossover(IList<PatternRecognitionNetwork> networks);
    }
}
