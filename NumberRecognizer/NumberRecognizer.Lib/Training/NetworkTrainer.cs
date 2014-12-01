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
    public class NetworkTrainer
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkTrainer"/> class.
        /// </summary>
        public NetworkTrainer()
        {
            InitializeParameters();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkTrainer" /> class.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        public NetworkTrainer(IEnumerable<TrainingImage> trainingData) : this()
        {
            TrainingData = new ConcurrentBag<TrainingImage>(trainingData);
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the parameters for the genetic algorithm.
        /// </summary>
        private void InitializeParameters()
        {
            PopulationSize = 100;
            MaxGenerations = 100;
            ImageHeight = 16;
            ImageWidth = 16;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the mutation instance.
        /// </summary>
        /// <value>
        /// The mutation instance.
        /// </value>
        public IMutation MutationInstance { get; set; }

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
        /// Gets or sets the size of the population.
        /// </summary>
        /// <value>
        /// The size of the population.
        /// </value>
        public int PopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum generations.
        /// </summary>
        /// <value>
        /// The maximum generations.
        /// </value>
        public int MaxGenerations { get; set; }

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight
        {
            get;
            private set;
        }

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
        public void AddTrainingData(TrainingImage trainingImage)
        {
            TrainingData.Add(trainingImage);

            ResultNetwork = null;
        }

        /// <summary>
        /// Adds the training data.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        public void AddTrainingData(ICollection<TrainingImage> trainingData)
        {
            foreach (TrainingImage item in trainingData)
            {
                TrainingData.Add(item);
            }

            ResultNetwork = null;
        }

        #endregion

        /// <summary>
        /// Trains the network.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PatternRecognitionNetwork> TrainNetwork()
        {
            if (MutationInstance == null || CrossoverInstance == null || SelectionInstance == null)
            {
                throw new InvalidOperationException("The Genetic algorithm parts (selection, mutation, crossover) " +
                                                    "are not correct initialized.");
            }

            SelectionInstance.PopulationSize = PopulationSize;

            IList<string> patterns = TrainingData.Select(x => x.RepresentingInformation).Distinct().ToList();
            ConcurrentBag<PatternRecognitionNetwork> currentGeneration = CreateInitialPopulation(patterns);
            List<PatternRecognitionNetwork> networks = currentGeneration.ToList();

            for (int i = 0; i < MaxGenerations; i++)
            {
                // Calculate Fitness
                Parallel.ForEach(currentGeneration, individual => individual.CalculateFitness(TrainingData.ToList()));

                PatternRecognitionNetwork bestNetwork = currentGeneration.OrderByDescending(x => x.Fitness).First();

                if (GenerationChanged != null)
                {
                    GenerationChanged(i, bestNetwork);
                }

                #region Obsolete  //TODO Remove
                // GUI Update
                //Dictionary<string, double> fitnessDetail = bestNetwork.GetFitnessDetail(TrainingData.ToList());

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                //{
                //    this.CurrentGenerationLabel.Content = i;
                //    this.CurrentFitnessLabel.Content = bestNetwork.Fitness.ToString("F8");

                //    for (int j = 0; j < fitnessDetail.Count; j++)
                //    {
                //        Label patternLabel = FindChild<Label>(this, string.Format("CurrentPattern{0}", j));
                //        patternLabel.Content = fitnessDetail.ToList()[j].Key;

                //        Label patternScoreLabel = FindChild<Label>(this, string.Format("CurrentPatternScore{0}", j));
                //        patternScoreLabel.Content = fitnessDetail.ToList()[j].Value.ToString("F8");
                //    }
                //}));

                #endregion

                ConcurrentBag<PatternRecognitionNetwork> newGeneration = new ConcurrentBag<PatternRecognitionNetwork>();

                // Specify individuals for recombination (truncation selection)
                IEnumerable<PatternRecognitionNetwork> selectionNetworks = SelectionInstance.ExecuteSelection(currentGeneration);

                // recombination / crossover - one point crossover
                IEnumerable<PatternRecognitionNetwork> recombinedNetworks = CrossoverInstance.ExecuteCrossover(selectionNetworks.ToList());

                Parallel.For(0, PopulationSize, (int x) =>
                {
                    PatternRecognitionNetwork parentNetwork =
                        recombinedNetworks.ToList().OrderBy(individual => Guid.NewGuid()).First();

                    // Create children
                    PatternRecognitionNetwork firstChild = new PatternRecognitionNetwork(ImageWidth, ImageHeight, patterns);

                    // Copy weights
                    for (int j = 0; j < parentNetwork.Genomes.Count; j++)
                    {
                        firstChild.Genomes[j].Weight = parentNetwork.Genomes[j].Weight;
                    }

                    //Uniform Mutation
                    firstChild = MutationInstance.ExecuteMutation(firstChild);
                    
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
