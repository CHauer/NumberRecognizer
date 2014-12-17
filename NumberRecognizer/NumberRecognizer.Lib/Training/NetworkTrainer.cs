//-----------------------------------------------------------------------
// <copyright file="NetworkTrainer.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>NetworkTrainer - Genection Algorithm Trainer.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Lib.Training
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumberRecognizer.Lib.Network;
    using NumberRecognizer.Lib.Training.Contract;
    using NumberRecognizer.Lib.Training.Events;

    /// <summary>
    /// The genetic algorithm trainer class.
    /// Provides functionality to train a neuronal network with 
    /// single or multiple generations.
    /// </summary>
    public class NetworkTrainer
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkTrainer"/> class.
        /// </summary>
        public NetworkTrainer()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkTrainer" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Wrong ImageHeight or Width of PatternTrainingImage</exception>
        public NetworkTrainer(IEnumerable<PatternTrainingImage> trainingData)
            : this(trainingData, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkTrainer" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Wrong ImageHeight or Width of PatternTrainingImage</exception>
        public NetworkTrainer(IEnumerable<PatternTrainingImage> trainingData, TrainingParameter parameter)
        {
            //parameter != null given parameters else standard parameters
            TrainingParameter = parameter ?? new TrainingParameter();

            if (trainingData != null)
            {
                TrainingData = new ConcurrentBag<PatternTrainingImage>(trainingData);
                ValidateTrainingData();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the training parameter.
        /// </summary>
        /// <value>
        /// The training parameter.
        /// </value>
        public TrainingParameter TrainingParameter { get; private set; }

        /// <summary>
        /// Gets the training data.
        /// </summary>
        /// <value>
        /// The training data.
        /// </value>
        public ConcurrentBag<PatternTrainingImage> TrainingData { get; private set; }

        /// <summary>
        /// Gets the result network.
        /// </summary>
        /// <value>
        /// The result network.
        /// </value>
        public PatternRecognitionNetwork ResultNetwork { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the generation has changed.
        /// </summary>
        public event EventHandler<GenerationChangedEventArgs> GenerationChanged;

        /// <summary>
        /// Occurs when the generation has changed.
        /// </summary>
        public event EventHandler<MultipleGenPoolGenerationChangedEventArgs> MultipleGenPoolGenerationChanged;

        /// <summary>
        /// Occurs when a new "best" network is calculated.
        /// Occurs on each generation change.
        /// </summary>
        public event EventHandler<PatternRecognitionNetwork> FittestNetworkChanged;

        #endregion

        #region Manage Training Data

        /// <summary>
        /// Adds the training data.
        /// </summary>
        /// <param name="patternTrainingImage">The training image.</param>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public void AddTrainingData(PatternTrainingImage patternTrainingImage)
        {
            TrainingData.Add(patternTrainingImage);

            ValidateTrainingData();

            ResultNetwork = null;
        }

        /// <summary>
        /// Adds the training data.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void AddTrainingData(ICollection<PatternTrainingImage> trainingData)
        {
            foreach (PatternTrainingImage item in trainingData)
            {
                TrainingData.Add(item);
            }

            ValidateTrainingData();

            ResultNetwork = null;
        }

        /// <summary>
        /// Validates the training data.
        /// </summary>
        /// <param name="checkTrainDataExists">if set to <c>true</c> [check train data exists].</param>
        /// <exception cref="System.ArgumentException">There is no valid TrainingData for the network training process.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The image height or width values of the
        /// training images do have differnt values.</exception>
        private void ValidateTrainingData(bool checkTrainDataExists = false)
        {
            if (TrainingData == null || TrainingData.Count == 0)
            {
                if (checkTrainDataExists)
                {
                    throw new ArgumentException("There is no valid TrainigData for the network training process.");
                }
                //else
                return;
            }

            if (TrainingData.Any(x => x.PixelValues.GetLength(0) != TrainingParameter.ImageHeight) ||
                TrainingData.Any(x => x.PixelValues.GetLength(1) != TrainingParameter.ImageWidth))
            {
                throw new ArgumentOutOfRangeException("The image height or width values of the " +
                                                      "training images do have differnt values.");
            }
        }

        #endregion

        /// <summary>
        /// Trains the network.
        /// </summary>
        /// <exception cref="System.ArgumentException" />
        /// <exception cref="System.ArgumentOutOfRangeException" />
        /// <returns>The trained network object.</returns>
        public IEnumerable<PatternRecognitionNetwork> TrainNetwork()
        {
            ConcurrentBag<PatternRecognitionNetwork> startPopulation = null;
            IList<PatternRecognitionNetwork> finalPopulation = null;

            ValidateTrainingData(checkTrainDataExists: true);
            TrainingParameter.CheckGeneticOperators();

            if (TrainingParameter.GenPoolTrainingMode == GenPoolType.MultipleGenPool)
            {
                startPopulation = CreateInitialPopulationFromMultiplePools();
            }

            //if startPopulation is null - Single Pool with random initialized population
            //else - Mutiple GenPool Merge Start Population
            GenPool finalPool = new GenPool(TrainingData, TrainingParameter, startPopulation);

            //Event forwarding
            finalPool.GenerationChanged += (sender, e) =>
            {
                if (GenerationChanged != null)
                {
                    GenerationChanged(this, e);
                }

                if (FittestNetworkChanged != null)
                {
                    FittestNetworkChanged(this, e.CurrentFittestNetwork);
                }
            };

            finalPopulation = finalPool.CalculatePoolGenerations();

            // Calculate Fitness
            Parallel.ForEach(finalPopulation, individual => individual.CalculateFitness(TrainingData.ToList()));

            ResultNetwork = finalPopulation.OrderBy(x => x.Fitness).First();

            return finalPopulation.ToList();
        }

        /// <summary>
        /// Creates the initial population for final calculation from multiple calculated pools.
        /// </summary>
        /// <returns>A initial population merged from multiple gen pools.</returns>
        private ConcurrentBag<PatternRecognitionNetwork> CreateInitialPopulationFromMultiplePools()
        {
            ConcurrentBag<PatternRecognitionNetwork> population = new ConcurrentBag<PatternRecognitionNetwork>();

            for (int i = 0; i < TrainingParameter.MultipleGenPoolCount; i++)
            {
                GenPool multiplePool = new GenPool(TrainingData, TrainingParameter) { IsMultipleGenPool = true };

                int multipleGenPoolIdentifier = i;

                multiplePool.GenerationChanged += (sender, e) =>
                {
                    if (MultipleGenPoolGenerationChanged != null)
                    {
                        MultipleGenPoolGenerationChanged(this, new MultipleGenPoolGenerationChangedEventArgs(e, multipleGenPoolIdentifier));
                    }

                    if (FittestNetworkChanged != null)
                    {
                        FittestNetworkChanged(this, e.CurrentFittestNetwork);
                    }
                };

                IList<PatternRecognitionNetwork> networks = multiplePool.CalculatePoolGenerations();

                Parallel.ForEach(networks, individual => individual.CalculateFitness(TrainingData.ToList()));

                networks.OrderByDescending(n => n.Fitness)
                        .Take(TrainingParameter.MultipleGenPoolSelectionCount)
                        .ToList()
                        .ForEach(population.Add);
            }

            return population;
        }

    }
}
