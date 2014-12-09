using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training.Contract;

namespace NumberRecognizer.Lib.Training
{
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
        public NetworkTrainer() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkTrainer" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Wrong ImageHeight or Width of TrainingImage</exception>
        public NetworkTrainer(IEnumerable<TrainingImage> trainingData) : this(trainingData, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkTrainer" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Wrong ImageHeight or Width of TrainingImage</exception>
        public NetworkTrainer(IEnumerable<TrainingImage> trainingData, TrainingParameter parameter)
        {
            //parameter != null 
            TrainingParameter = parameter ?? new TrainingParameter();

            if (trainingData != null)
            {
                TrainingData = new ConcurrentBag<TrainingImage>(trainingData);
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
        /// Gets or sets the mutation instance.
        /// </summary>
        /// <value>
        /// The mutation instance.
        /// </value>
        public IList<IMutation> MutationInstances { get; set; }

        /// <summary>
        /// Gets or sets the selection instance.
        /// </summary>
        /// <value>
        /// The selection instance.
        /// </value>
        public ISelection SelectionInstance { get; set; }

        /// <summary>
        /// Gets or sets the crossover instance.
        /// </summary>
        /// <value>
        /// The crossover instance.
        /// </value>
        public ICrossover CrossoverInstance { get; set; }

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

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the generation has changed.
        /// </summary>
        public event Action<int, PatternRecognitionNetwork> GenerationChanged;

        #endregion

        #region Manage Training Data

        /// <summary>
        /// Adds the training data.
        /// </summary>
        /// <param name="trainingImage">The training image.</param>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public void AddTrainingData(TrainingImage trainingImage)
        {
            TrainingData.Add(trainingImage);

            ValidateTrainingData();

            ResultNetwork = null;
        }

        /// <summary>
        /// Adds the training data.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void AddTrainingData(ICollection<TrainingImage> trainingData)
        {
            foreach (TrainingImage item in trainingData)
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
        /// <exception cref="System.ArgumentException">There is no valid TrainigData for the network training process.</exception>
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
        /// <returns></returns>
        public IEnumerable<PatternRecognitionNetwork> TrainNetwork()
        {
            IList<string> patterns = null;
            ConcurrentBag<PatternRecognitionNetwork> currentGeneration = null;

            ValidateTrainingData(checkTrainDataExists:true);
            CheckGeneticOperators();

            patterns = TrainingData.Select(x => x.RepresentingInformation).Distinct().ToList();

            if (TrainingParameter.GenPoolTrainingMode == GenPoolType.SingleGenPool)
            {
                currentGeneration = CreateInitialPopulation(patterns);
            }
            else
            {
                //TODO Temp change to multiple
                currentGeneration = CreateInitialPopulation(patterns);
                //currentGeneration = CreateInitialPopulationFromMultiplePools(patterns);
            }

            for (int i = 0; i < TrainingParameter.MaxGenerations; i++)
            {
                // Calculate Fitness
                Parallel.ForEach(currentGeneration, individual => individual.CalculateFitness(TrainingData.ToList()));

                PatternRecognitionNetwork bestNetwork = currentGeneration.OrderByDescending(x => x.Fitness).First();

                if (GenerationChanged != null)
                {
                    GenerationChanged(i, bestNetwork);
                }

                ConcurrentBag<PatternRecognitionNetwork> newGeneration = new ConcurrentBag<PatternRecognitionNetwork>();

                // Specify individuals for recombination (e.g. truncation selection)
                IEnumerable<PatternRecognitionNetwork> selectionNetworks = SelectionInstance.ExecuteSelection(currentGeneration);

                // recombination / crossover (e.g. uniform crossover)
                IEnumerable<PatternRecognitionNetwork> recombinedNetworks = CrossoverInstance.ExecuteCrossover(selectionNetworks.ToList());

                Parallel.For(0, TrainingParameter.PopulationSize, (int x) =>
                {
                    PatternRecognitionNetwork parentNetwork =
                        recombinedNetworks.ToList().OrderBy(individual => Guid.NewGuid()).First();

                    // Create children
                    PatternRecognitionNetwork firstChild = new PatternRecognitionNetwork(TrainingParameter.ImageWidth, TrainingParameter.ImageHeight, patterns);

                    // Copy weights
                    firstChild.CopyGenomeWeights(parentNetwork);

                    //Uniform Mutation
                    firstChild = ChooseMutation(parentNetwork.Fitness).ExecuteMutation(firstChild);
                    
                    // Add new children
                    newGeneration.Add(firstChild);
                });

                currentGeneration = newGeneration;
            }

            // Calculate Fitness
            Parallel.ForEach(currentGeneration, individual => individual.CalculateFitness(TrainingData.ToList()));

            ResultNetwork = currentGeneration.OrderBy(x => x.Fitness).First();

            return currentGeneration.ToList();
        }

        /// <summary>
        /// Chooses the mutation instance based on the current network fitness.
        /// Search for the instances where the fitness is greater than the minNetworkFitness 
        /// of the instance and take the instance with the maximun minNetworkFitness.
        /// </summary>
        /// <param name="networkFitness">The network fitness.</param>
        /// <returns></returns>
        private IMutation ChooseMutation(double networkFitness)
        {
            return MutationInstances.Where(mi => networkFitness > mi.MinNetworkFitness)
                                    .OrderByDescending(mi => mi.MinNetworkFitness)
                                    .First();
        }

        /// <summary>
        /// Initializes the genetic operators.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The Genetic algorithm parts (selection, mutation, crossover)  +
        ///                                                     are not correct initialized.</exception>
        private void CheckGeneticOperators()
        {
            if (MutationInstances == null || MutationInstances.Count == 0 ||
                CrossoverInstance == null || SelectionInstance == null)
            {
                throw new InvalidOperationException("The Genetic algorithm parts (selection, mutation, crossover) " +
                                                    "are not correct initialized.");
            }
        }

        /// <summary>
        /// Creates the initial population.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <returns></returns>
        private ConcurrentBag<PatternRecognitionNetwork> CreateInitialPopulation(IEnumerable<string> patterns)
        {
            ConcurrentBag<PatternRecognitionNetwork> currentGeneration =
                new ConcurrentBag<PatternRecognitionNetwork>(new List<PatternRecognitionNetwork>());

            if (currentGeneration.Count == 0)
            {
                Parallel.For(0, PopulationSize, (int i) =>
                {
                    ThreadSafeRandom random = new ThreadSafeRandom();

                    PatternRecognitionNetwork patternRecognitionNetwork = new PatternRecognitionNetwork(ImageWidth, ImageHeight,
                        patterns);
                    patternRecognitionNetwork.Genomes.ForEach(x => x.Weight = (2*random.NextDouble()) - 1);

                    // ReSharper disable once AccessToModifiedClosure
                    currentGeneration.Add(patternRecognitionNetwork);
                });
            }

            return currentGeneration;
        }
    }
}
