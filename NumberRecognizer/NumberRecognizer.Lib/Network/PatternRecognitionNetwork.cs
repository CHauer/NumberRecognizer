using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NumberRecognizer.Lib.Network
{
    /// <summary>
    /// The neuronal network class with input, middle/hidden and output layer.
    /// </summary>
    [Serializable]
	public class PatternRecognitionNetwork
    {
        /// <summary>
        /// The synchronize lock
        /// </summary>
        [NonSerialized]
        private object syncLock = new object();

        /// <summary>
        /// The fitness of the network
        /// </summary>
        private double fitness;

        /// <summary>
        /// The input width
        /// </summary>
        private int inputWidth;

        /// <summary>
        /// The input height
        /// </summary>
        private int inputHeight;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternRecognitionNetwork"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="hiddenLayerType">Type of the hidden layer.</param>
        public PatternRecognitionNetwork(int width, int height, IEnumerable<string> patterns,
                                            HiddenLayerType hiddenLayerType = HiddenLayerType.OverlayedLining | HiddenLayerType.OverlayedBoxing)
		{
            InitializeNetworkParameters(width, height, patterns, hiddenLayerType);

            CreateNetworkLayers();
		}

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the network.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="hiddenLayerType">Type of the hidden layer.</param>
        /// <exception cref="System.ArgumentException">The Height or Width parameters of the Network are invalid.</exception>
        private void InitializeNetworkParameters(int width, int height, IEnumerable<string> patterns, HiddenLayerType hiddenLayerType)
	    {
	        const int divChecker = 2;

	        if (height % divChecker != 0 || width % divChecker != 0)
	        {
	            throw new ArgumentException("The Height or Width parameters of the Network are invalid.");
	        }

            HiddenLayerMode = hiddenLayerType;
	        InputHeight = height;
	        InputWidth = width;
            Patterns = new List<string>(patterns);
	    }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fitness.
        /// </summary>
        /// <value>
        /// The fitness.
        /// </value>
        public double Fitness
        {
            get { return fitness; }
            private set { fitness = value; }
        }

        /// <summary>
        /// Gets or sets the genomes.
        /// </summary>
        /// <value>
        /// The genomes.
        /// </value>
        public List<WeightedLink> Genomes { get; set; }

        /// <summary>
        /// Gets or sets the input neurons.
        /// </summary>
        /// <value>
        /// The input neurons.
        /// </value>
		public InputNeuron[,] InputNeurons { get; set; }

        /// <summary>
        /// Gets or sets the output neurons.
        /// </summary>
        /// <value>
        /// The output neurons.
        /// </value>
		public Dictionary<string, OutputNeuron> OutputNeurons { get; set; }

        /// <summary>
        /// Gets the hidden layer mode.
        /// </summary>
        /// <value>
        /// The hidden layer mode.
        /// </value>
        public HiddenLayerType HiddenLayerMode{get; private set;}

        /// <summary>
        /// Gets the width of the input.
        /// </summary>
        /// <value>
        /// The width of the input.
        /// </value>
        public int InputWidth
        {
            get { return inputWidth; }
            private set { inputWidth = value; }
        }

        /// <summary>
        /// Gets the height of the input.
        /// </summary>
        /// <value>
        /// The height of the input.
        /// </value>
        public int InputHeight
        {
            get { return inputHeight; }
            private set { inputHeight = value; }
        }

        /// <summary>
        /// Gets or sets the hidden neurons.
        /// </summary>
        /// <value>
        /// The hidden neurons.
        /// </value>
        public HashSet<HiddenNeuron> HiddenNeurons { get; set; }

        /// <summary>
        /// Gets or sets the patterns.
        /// </summary>
        /// <value>
        /// The patterns.
        /// </value>
        public List<string> Patterns { get; set; }

        #endregion

        #region Create

        /// <summary>
        /// Creates the specified width.
        /// </summary>
        private void CreateNetworkLayers()
        {
            // Initialize Genomes
            Genomes = new List<WeightedLink>();

            //Create input layer with input neurons
            CreateInputLayer();

            //Add middle or hidden layer
            CreateHiddenLayer();

            //Create output layer with output neurons from patterns
            CreateOutputLayer();
        }

        /// <summary>
        /// Creates the input layer.
        /// </summary>
        private void CreateInputLayer()
        {
            // Input Neurons
            InputNeurons = new InputNeuron[InputWidth, InputHeight];

            for (int i = 0; i < InputWidth; i++)
            {
                for (int j = 0; j < InputHeight; j++)
                {
                    InputNeurons[i, j] = new InputNeuron();
                }
            }
        }

        /// <summary>
        /// Creates the hidden layer.
        /// </summary>
        private void CreateHiddenLayer()
        {
            //Hidden/Middle Neurons
            HiddenNeurons = new HashSet<HiddenNeuron>();

            if (HiddenLayerMode.HasFlag(HiddenLayerType.Lining) ||
                HiddenLayerMode.HasFlag(HiddenLayerType.OverlayedLining))
            {
                int moveIncrementer = 1;

                if (HiddenLayerMode.HasFlag(HiddenLayerType.Lining))
                {
                    moveIncrementer = 2;
                }
                else if (HiddenLayerMode.HasFlag(HiddenLayerType.OverlayedLining))
                {
                    moveIncrementer = 1;
                }

                //Divide the input matrix in horizontal and vertical lines with height or width of 2 InputNeurons

                //top down lines and "height" of 2
                for (int y = 0; y < InputHeight - 1; y += moveIncrementer)
                {
                    HiddenNeuron hiddenNeuron = new HiddenNeuron();

                    for (int x = 0; x < InputWidth - 1; x++)
                    {
                        WeightedLink weightedLinkA = new WeightedLink() { Neuron = InputNeurons[x, y] };
                        WeightedLink weightedLinkB = new WeightedLink() { Neuron = InputNeurons[x, y + 1] };

                        hiddenNeuron.InputLayer.Add(weightedLinkA);
                        hiddenNeuron.InputLayer.Add(weightedLinkB);

                        // Add to Genomes
                        Genomes.Add(weightedLinkA);
                        Genomes.Add(weightedLinkB);
                    }

                    HiddenNeurons.Add(hiddenNeuron);
                }

                //left right lines and "width" of 2
                for (int x = 0; x < InputWidth - 1; x += moveIncrementer)
                {
                    HiddenNeuron hiddenNeuron = new HiddenNeuron();

                    for (int y = 0; y < InputHeight - 1; y++)
                    {
                        WeightedLink weightedLinkA = new WeightedLink() { Neuron = InputNeurons[x, y] };
                        WeightedLink weightedLinkB = new WeightedLink() { Neuron = InputNeurons[x + 1, y] };

                        hiddenNeuron.InputLayer.Add(weightedLinkA);
                        hiddenNeuron.InputLayer.Add(weightedLinkB);

                        // Add to Genomes
                        Genomes.Add(weightedLinkA);
                        Genomes.Add(weightedLinkB);
                    }

                    HiddenNeurons.Add(hiddenNeuron);
                }
            }


            if (HiddenLayerMode.HasFlag(HiddenLayerType.Boxing) ||
                HiddenLayerMode.HasFlag(HiddenLayerType.OverlayedBoxing))
            {
                int moveIncrementer = 1;

                if (HiddenLayerMode.HasFlag(HiddenLayerType.Boxing))
                {
                    moveIncrementer = 2;
                }
                else if (HiddenLayerMode.HasFlag(HiddenLayerType.OverlayedBoxing))
                {
                    moveIncrementer = 1;
                }

                //Divide the input matrix into boxes of 4 InputNeurons
                for (int x = 0; x < InputWidth - 1; x += moveIncrementer)
                {
                    for (int y = 0; y < InputHeight - 1; y += moveIncrementer)
                    {
                        HiddenNeuron hiddenNeuron = new HiddenNeuron();

                        WeightedLink weightedLinkLo = new WeightedLink() { Neuron = InputNeurons[x, y] };
                        WeightedLink weightedLinkLu = new WeightedLink() { Neuron = InputNeurons[x, y + 1] };
                        WeightedLink weightedLinkRo = new WeightedLink() { Neuron = InputNeurons[x + 1, y] };
                        WeightedLink weightedLinkRu = new WeightedLink() { Neuron = InputNeurons[x + 1, y + 1] };

                        hiddenNeuron.InputLayer.AddRange(new List<WeightedLink>() { weightedLinkLo, weightedLinkLu,
                                                                                    weightedLinkRo, weightedLinkRu});

                        Genomes.Add(weightedLinkLo);
                        Genomes.Add(weightedLinkLu);
                        Genomes.Add(weightedLinkRo);
                        Genomes.Add(weightedLinkRu);

                        HiddenNeurons.Add(hiddenNeuron);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the output layer.
        /// </summary>
        private void CreateOutputLayer()
        {
            // Create Output Neurons
            OutputNeurons = new Dictionary<string, OutputNeuron>();

            Patterns.ForEach(x =>
            {
                OutputNeuron outputNeuron = new OutputNeuron();

                foreach (HiddenNeuron hiddenNeuron in HiddenNeurons)
                {
                    WeightedLink weightedLink = new WeightedLink() { Neuron = hiddenNeuron };

                    outputNeuron.InputLayer.Add(weightedLink);

                    // Add to Genomes
                    Genomes.Add(weightedLink);
                }

                OutputNeurons.Add(x, outputNeuron);
            });
        }

        #endregion

        /// <summary>
        /// Calculates the fitness.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        public void CalculateFitness(ICollection<TrainingImage> trainingData)
		{
			double sum = 0;

			foreach (TrainingImage image in trainingData)
			{
				SetInputData(image.PixelValues);

                //solve to: access to foreach variable in closure / function
                var pattern = image.RepresentingInformation;

				IEnumerable<double> values =
                    OutputNeurons.Where(x => x.Key != pattern).Select(x => x.Value.ActivationValue);

				sum += 1 - values.Average();

				sum += 1 + OutputNeurons[image.RepresentingInformation].ActivationValue;
			}

			Fitness = sum / (trainingData.Count * 4);
		}

        /// <summary>
        /// Gets the fitness detail.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        /// <returns></returns>
		public Dictionary<string, double> GetFitnessDetail(ICollection<TrainingImage> trainingData)
		{
			Dictionary<string, double> results = new Dictionary<string, double>();

			foreach (
				IGrouping<string, TrainingImage> representingInformation in trainingData.GroupBy(x => x.RepresentingInformation))
			{
				results[representingInformation.Key] = 0;

				foreach (TrainingImage image in representingInformation)
				{
					SetInputData(image.PixelValues);

                    //solve to: access to foreach variable in closure / function
                    var pattern = image.RepresentingInformation;

					IEnumerable<double> values =
						OutputNeurons.Where(x => x.Key != pattern).Select(x => x.Value.ActivationValue);

					results[representingInformation.Key] += 1 - values.Average();
					results[representingInformation.Key] += 1 + OutputNeurons[image.RepresentingInformation].ActivationValue;
				}

				results[representingInformation.Key] /= representingInformation.Count() * 4;
			}

			return results;
		}

        /// <summary>
        /// Recognizes the character.
        /// </summary>
        /// <param name="pixelValues">The pixel values.</param>
        /// <returns></returns>
		public IEnumerable<RecognitionResult> RecognizeCharacter(double[,] pixelValues)
		{
			SetInputData(pixelValues);

			return
				OutputNeurons.Select(
					outputNeuron =>
						new RecognitionResult()
						{
							Propability = outputNeuron.Value.ActivationValue,
							RecognizedCharacter = outputNeuron.Key
						});
		}

        /// <summary>
        /// Sets the input data.
        /// </summary>
        /// <param name="pixelValues">The pixel values.</param>
		private void SetInputData(double[,] pixelValues)
		{
            //reset cached values - Hidden layer
            ResetHiddenLayerCache();
            //reset cached values - Output layer
            ResetOutputLayerCache();

            //set new input value to input layer
			for (int i = 0; i < pixelValues.GetLength(0); i++)
			{
				for (int j = 0; j < pixelValues.GetLength(1); j++)
				{
					InputNeurons[i, j].Value = pixelValues[i, j];
				}
			}
		}

        /// <summary>
        /// Resets the hidden layer cache.
        /// </summary>
        private void ResetHiddenLayerCache()
        {
            foreach (ICacheable neuron in HiddenNeurons)
            {
                neuron.ResetCachedValue();
            }
        }

        /// <summary>
        /// Resets the output layer cache.
        /// </summary>
        private void ResetOutputLayerCache()
        {
            foreach (ICacheable neuron in OutputNeurons.Values)
            {
                neuron.ResetCachedValue();
            }
        }
    }
}