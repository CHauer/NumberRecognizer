//-----------------------------------------------------------------------
// <copyright file="GenPool.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>GenPool - Random Helper.</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training.Events;

namespace NumberRecognizer.Lib.Training
{
    /// <summary>
    /// Represents a GenPool with a variable generation count
    /// and a variable population size.
    /// Offers functionality to calculate a n generations of network populations.
    /// </summary>
    public class GenPool
    {
        /// <summary>
        ///     The current generation
        /// </summary>
        private ConcurrentBag<PatternRecognitionNetwork> currentGeneration;

        #region Event

        /// <summary>
        ///     Occurs when the generation has changed.
        /// </summary>
        public event EventHandler<GenerationChangedEventArgs> GenerationChanged;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenPool" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <param name="parameter">The parameter.</param>
        public GenPool(IEnumerable<PatternTrainingImage> trainingData,
            TrainingParameter parameter)
            : this(trainingData, parameter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenPool" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="startPopulation">The start population.</param>
        public GenPool(IEnumerable<PatternTrainingImage> trainingData,
            TrainingParameter parameter,
            ConcurrentBag<PatternRecognitionNetwork> startPopulation)
        {
            IsMultipleGenPool = false;
            TrainingParameter = parameter;

            TrainingData = new ConcurrentBag<PatternTrainingImage>(trainingData);
            Patterns = TrainingData.Select(x => x.RepresentingInformation).Distinct().ToList();

            //if startPopulation is null create random initial population
            currentGeneration = startPopulation ?? CreateInitialPopulation();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating this GenPool
        ///     instance is a multiple gen pool instance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the multiple gen pool training parameters.
        ///     should be used; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultipleGenPool { get; set; }

        /// <summary>
        ///     Gets the result networks.
        /// </summary>
        /// <value>
        ///     The result networks.
        /// </value>
        public ICollection<PatternRecognitionNetwork> ResultNetworks { get; private set; }

        /// <summary>
        ///     Gets the training parameter.
        /// </summary>
        /// <value>
        ///     The training parameter.
        /// </value>
        public TrainingParameter TrainingParameter { get; set; }

        /// <summary>
        ///     Gets the training data.
        /// </summary>
        /// <value>
        ///     The training data.
        /// </value>
        public ConcurrentBag<PatternTrainingImage> TrainingData { get; private set; }

        /// <summary>
        ///     Gets the result network.
        /// </summary>
        /// <value>
        ///     The result network.
        /// </value>
        public PatternRecognitionNetwork ResultNetwork { get; private set; }

        /// <summary>
        ///     Gets the patterns.
        /// </summary>
        /// <value>
        ///     The patterns.
        /// </value>
        public List<string> Patterns { get; private set; }

        #endregion

        /// <summary>
        /// Creates the initial population.
        /// </summary>
        /// <returns>Bag of networks.</returns>
        private ConcurrentBag<PatternRecognitionNetwork> CreateInitialPopulation()
        {
            var populationSize = IsMultipleGenPool
                ? TrainingParameter.MultipleGenPoolPopulationSize
                : TrainingParameter.SingleGenPoolPopulationSize;

            var generation =
                new ConcurrentBag<PatternRecognitionNetwork>(new List<PatternRecognitionNetwork>());

            Parallel.For(0, populationSize, (int i) =>
            {
                var random = new ThreadSafeRandom();

                var patternRecognitionNetwork = new PatternRecognitionNetwork(TrainingParameter.ImageWidth,
                    TrainingParameter.ImageHeight,
                    Patterns);
                patternRecognitionNetwork.Genomes.ForEach(x => x.Weight = (2 * random.NextDouble()) - 1);

                // ReSharper disable once AccessToModifiedClosure
                generation.Add(patternRecognitionNetwork);
            });

            return generation;
        }

        /// <summary>
        ///     Calculates the pool.
        ///     Calculates Generations to the in the Training Parameter
        ///     defined maxGenerations counter.
        /// </summary>
        /// <returns>List of network.</returns>
        public IList<PatternRecognitionNetwork> CalculatePoolGenerations()
        {
            var populationSize = IsMultipleGenPool
                ? TrainingParameter.MultipleGenPoolPopulationSize
                : TrainingParameter.SingleGenPoolPopulationSize;
            var maxGenerations = IsMultipleGenPool
                ? TrainingParameter.MultipleGenPoolMaxGenerations
                : TrainingParameter.SingleGenPoolMaxGenerations;

            for (var i = 0; i < maxGenerations; i++)
            {
                // Calculate Fitness
                Parallel.ForEach(currentGeneration, individual => individual.CalculateFitness(TrainingData.ToList()));

                var bestNetwork = currentGeneration.OrderByDescending(x => x.Fitness).First();

                if (GenerationChanged != null)
                {
                    GenerationChanged(this,
                        new GenerationChangedEventArgs(i, bestNetwork,
                            bestNetwork.GetFitnessDetail(TrainingData.ToList())));
                }

                var newGeneration = new ConcurrentBag<PatternRecognitionNetwork>();

                // Specify individuals for recombination (e.g. truncation selection)
                var selectionNetworks = TrainingParameter.SelectionInstance.ExecuteSelection(currentGeneration);

                // recombination / crossover (e.g. uniform crossover)
                var recombinedNetworks = TrainingParameter.CrossoverInstance.ExecuteCrossover(selectionNetworks.ToList());

                Parallel.For(0, populationSize, (int x) =>
                {
                    var parentNetwork =
                        recombinedNetworks.ToList().OrderBy(individual => Guid.NewGuid()).First();

                    // Create children
                    var firstChild = new PatternRecognitionNetwork(TrainingParameter.ImageWidth,
                        TrainingParameter.ImageHeight,
                        Patterns);

                    // Copy weights
                    firstChild.CopyGenomeWeights(parentNetwork);

                    //Uniform Mutation
                    firstChild = TrainingParameter.ChooseMutation(bestNetwork.Fitness).ExecuteMutation(firstChild);

                    // Add new children
                    newGeneration.Add(firstChild);
                });

                currentGeneration = newGeneration;
            }

            return currentGeneration.ToList();
        }
    }
}