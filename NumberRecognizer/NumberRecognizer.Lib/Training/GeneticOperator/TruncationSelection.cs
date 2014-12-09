using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training.Contract;

namespace NumberRecognizer.Lib.Training.GeneticOperator
{
    public class TruncationSelection : ISelection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TruncationSelection"/> class.
        /// </summary>
        public TruncationSelection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TruncationSelection"/> class.
        /// </summary>
        /// <param name="truncationSelectionPercentage">The truncation selection percentage.</param>
        /// <param name="populationSize">Size of the population.</param>
        public TruncationSelection(double truncationSelectionPercentage, int populationSize)
        {
            this.PopulationSize = populationSize; 
            this.TruncationSelectionPercentage = truncationSelectionPercentage;
        }

        /// <summary>
        /// Gets or sets the truncation selection percentage.
        /// </summary>
        /// <value>
        /// The truncation selection percentage.
        /// </value>
        public double TruncationSelectionPercentage { get; set; }

        /// <summary>
        /// Gets or sets the size of the population.
        /// </summary>
        /// <value>
        /// The size of the population.
        /// </value>
        public int PopulationSize { get; set; }

        /// <summary>
        /// Gets the truncation selection absolute.
        /// </summary>
        /// <value>
        /// The truncation selection absolute.
        /// </value>
        public int TruncationSelectionAbsolute
        {
            get
            {
                return (int)(PopulationSize * TruncationSelectionPercentage);
            }
        }

        /// <summary>
        /// Executes the selection.
        /// Specifies individuals for recombination (truncation selection)
        /// </summary>
        /// <param name="currentGeneration">The current generation.</param>
        /// <returns></returns>
        public IEnumerable<PatternRecognitionNetwork> ExecuteSelection(ConcurrentBag<PatternRecognitionNetwork> currentGeneration)
        {
            PopulationSize = currentGeneration.Count;

            IEnumerable<PatternRecognitionNetwork> selectionNetworks =
                currentGeneration.OrderByDescending(x => x.Fitness).Take(TruncationSelectionAbsolute).ToList();

            return selectionNetworks;
        }

    }
}
