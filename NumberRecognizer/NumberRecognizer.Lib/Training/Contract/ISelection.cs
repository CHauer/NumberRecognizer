using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

namespace NumberRecognizer.Lib.Training.Contract
{
    /// <summary>
    /// The Interface for the Selection Genetic Operator for the genetic algorithm.
    /// </summary>
    public interface ISelection
    {

        /// <summary>
        /// Executes the selection operator.
        /// </summary>
        /// <param name="currentGeneration">The current generation.</param>
        /// <returns></returns>
        IEnumerable<PatternRecognitionNetwork> ExecuteSelection(ConcurrentBag<PatternRecognitionNetwork> currentGeneration);
    }
}
