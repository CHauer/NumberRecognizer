using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.Lib.Network;

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
            TruncationSelectionPercentage = 0.1;
            ImageHeight = 16;
            ImageWidth = 16;
        }

        #endregion

        #region Properties

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
        /// Gets or sets the truncation selection percentage.
        /// </summary>
        /// <value>
        /// The truncation selection percentage.
        /// </value>
        public double TruncationSelectionPercentage { get; set; }

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
        /// Occurs when [generation changed].
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
            IEnumerable<string> patterns = TrainingData.Select(x => x.RepresentingInformation).Distinct();

            // Create initial population
            ConcurrentBag<PatternRecognitionNetwork> currentGeneration =
                new ConcurrentBag<PatternRecognitionNetwork>(new List<PatternRecognitionNetwork>());

            if (currentGeneration.Count == 0)
            {
                Parallel.For(0, PopulationSize, (int i) =>
                {
                    ThreadSafeRandom random = new ThreadSafeRandom();

                    PatternRecognitionNetwork patternRecognitionNetwork = new PatternRecognitionNetwork(ImageWidth, ImageHeight, patterns);
                    patternRecognitionNetwork.Genomes.ForEach(x => x.Weight = (2 * random.NextDouble()) - 1);

                    currentGeneration.Add(patternRecognitionNetwork);
                });
            }

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
                IEnumerable<PatternRecognitionNetwork> patternRecognitionNetworks =
                    currentGeneration.OrderByDescending(x => x.Fitness).Take(TruncationSelectionAbsolute);

                // TODO: Add recombination / crossover


                Parallel.For(0, PopulationSize, (int x) =>
                {
                    ThreadSafeRandom random = new ThreadSafeRandom();

                    PatternRecognitionNetwork patternRecognitionNetwork =
                        patternRecognitionNetworks.ToList().OrderBy(individual => Guid.NewGuid()).First();

                    // Create children
                    PatternRecognitionNetwork firstChild = new PatternRecognitionNetwork(ImageWidth, ImageHeight, patterns);

                    // Copy weights
                    for (int j = 0; j < patternRecognitionNetwork.Genomes.Count; j++)
                    {
                        firstChild.Genomes[j].Weight = patternRecognitionNetwork.Genomes[j].Weight;
                    }

                    // Simple random mutation
                    firstChild.Genomes.ForEach(genome =>
                    {
                        if (random.NextDouble() > 0.95)
                        {
                            genome.Weight += (random.NextDouble() * 2) - 1;
                        }
                    });

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

    }
}
