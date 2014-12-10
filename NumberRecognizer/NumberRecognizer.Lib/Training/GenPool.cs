using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// The current generation
        /// </summary>
        private ConcurrentBag<PatternRecognitionNetwork> currentGeneration;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenPool"/> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <param name="parameter">The parameter.</param>
        public GenPool(IEnumerable<TrainingImage> trainingData,
                       TrainingParameter parameter)
            : this(trainingData, parameter, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenPool" /> class.
        /// </summary>
        /// <param name="startPopulation">The start generation.</param>
        /// <param name="trainingData">The training data.</param>
        /// <param name="parameter">The parameter.</param>
        public GenPool(IEnumerable<TrainingImage> trainingData,
                       TrainingParameter parameter,
                       ConcurrentBag<PatternRecognitionNetwork> startPopulation)
        {
            IsMultipleGenPool = false;
            TrainingParameter = parameter;

            TrainingData = new ConcurrentBag<TrainingImage>(trainingData);
            Patterns = TrainingData.Select(x => x.RepresentingInformation).Distinct().ToList();

            //if startPopulation is null create random initial population
            currentGeneration = startPopulation ?? CreateInitialPopulation();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating this genpool 
        /// instance is a multiple gen pool instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the multiple gen pool training pararmeters 
        ///   should be used; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultipleGenPool { get; set; }

        /// <summary>
        /// Gets the result networks.
        /// </summary>
        /// <value>
        /// The result networks.
        /// </value>
        public ICollection<PatternRecognitionNetwork> ResultNetworks { get; private set; }

        /// <summary>
        /// Gets the training parameter.
        /// </summary>
        /// <value>
        /// The training parameter.
        /// </value>
        public TrainingParameter TrainingParameter { get; set; }

        /// <summary>
        /// Gets the training data.
        /// </summary>
        /// <value>
        /// The training data.
        /// </value>
        public ConcurrentBag<TrainingImage> TrainingData { get; private set; }

        /// <summary>
        /// Gets the result network.
        /// </summary>
        /// <value>
        /// The result network.
        /// </value>
        public PatternRecognitionNetwork ResultNetwork { get; private set; }

        /// <summary>
        /// Gets the patterns.
        /// </summary>
        /// <value>
        /// The patterns.
        /// </value>
        public List<string> Patterns { get; private set; }

        #endregion

        #region Event

        /// <summary>
        /// Occurs when the generation has changed.
        /// </summary>
        public event EventHandler<GenerationChangedEventArgs> GenerationChanged;

        #endregion

        /// <summary>
        /// Creates the initial population.
        /// </summary>
        /// <returns></returns>
        private ConcurrentBag<PatternRecognitionNetwork> CreateInitialPopulation()
        {
            int populationSize = IsMultipleGenPool ? TrainingParameter.MultipleGenPoolPopulationSize 
                                                  : TrainingParameter.SingleGenPoolPopulationSize;
 
            ConcurrentBag<PatternRecognitionNetwork> generation =
                new ConcurrentBag<PatternRecognitionNetwork>(new List<PatternRecognitionNetwork>());

            Parallel.For(0, populationSize, (int i) =>
            {
                ThreadSafeRandom random = new ThreadSafeRandom();

                PatternRecognitionNetwork patternRecognitionNetwork = new PatternRecognitionNetwork(TrainingParameter.ImageWidth,
                                                                                                    TrainingParameter.ImageHeight,
                                                                                                    Patterns);
                patternRecognitionNetwork.Genomes.ForEach(x => x.Weight = (2 * random.NextDouble()) - 1);

                // ReSharper disable once AccessToModifiedClosure
                generation.Add(patternRecognitionNetwork);
            });

            return generation;
        }

        /// <summary>
        /// Calculates the pool.
        /// Calculates Generations to the in the Traingparameter
        /// definied maxGenerations counter.
        /// </summary>
        /// <returns></returns>
        public IList<PatternRecognitionNetwork> CalculatePoolGenerations()
        {
            int populationSize = IsMultipleGenPool ? TrainingParameter.MultipleGenPoolPopulationSize
                                                  : TrainingParameter.SingleGenPoolPopulationSize;
            int maxGenerations = IsMultipleGenPool ? TrainingParameter.MultipleGenPoolMaxGenerations
                                                  : TrainingParameter.SingleGenPoolMaxGenerations;

            for (int i = 0; i < maxGenerations; i++)
            {
                // Calculate Fitness
                Parallel.ForEach(currentGeneration, individual => individual.CalculateFitness(TrainingData.ToList()));

                PatternRecognitionNetwork bestNetwork = currentGeneration.OrderByDescending(x => x.Fitness).First();

                if (GenerationChanged != null)
                {
                    GenerationChanged(this, new GenerationChangedEventArgs(i, bestNetwork));
                }

                ConcurrentBag<PatternRecognitionNetwork> newGeneration = new ConcurrentBag<PatternRecognitionNetwork>();

                // Specify individuals for recombination (e.g. truncation selection)
                IEnumerable<PatternRecognitionNetwork> selectionNetworks = TrainingParameter.SelectionInstance.ExecuteSelection(currentGeneration);

                // recombination / crossover (e.g. uniform crossover)
                IEnumerable<PatternRecognitionNetwork> recombinedNetworks = TrainingParameter.CrossoverInstance.ExecuteCrossover(selectionNetworks.ToList());

                Parallel.For(0, populationSize, (int x) =>
                {
                    PatternRecognitionNetwork parentNetwork =
                        recombinedNetworks.ToList().OrderBy(individual => Guid.NewGuid()).First();

                    // Create children
                    PatternRecognitionNetwork firstChild = new PatternRecognitionNetwork(TrainingParameter.ImageWidth, 
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
