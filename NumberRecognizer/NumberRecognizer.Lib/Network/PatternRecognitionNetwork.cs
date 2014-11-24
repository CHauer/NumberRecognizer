using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberRecognizer.Lib.Network
{
    /// <summary>
    /// The neuronal network class with input, middle/hidden and output layer.
    /// </summary>
	public class PatternRecognitionNetwork
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternRecognitionNetwork"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="hiddenLayerType">Type of the hidden layer.</param>
        public PatternRecognitionNetwork(int width, int height, IEnumerable<string> patterns,
                                            HiddenLayerType hiddenLayerType = HiddenLayerType.Lining)
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
	        int divChecker = 2;

	        if (hiddenLayerType == HiddenLayerType.Boxing)
	        {
	            divChecker = 4;
	        }

	        if (height%divChecker != 0 || width%divChecker != 0)
	        {
	            throw new ArgumentException("The Height or Width parameters of the Network are invalid.");
	        }

            HiddenLayerMode = hiddenLayerType;
	        InputHeight = height;
	        InputWidth = width;
            Patterns = new List<string>(patterns);
	    }

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

        private void CreateHiddenLayer()
        {
            //Hidden/Middle Neurons
            HiddenNeurons = new List<HiddenNeuron>();

            if(HiddenLayerMode == HiddenLayerType.Lining)
            {
                //Divide the input matrix in horizontal and vertical lines with height or width of 2 InputNeurons

                //top down lines and "height" of 2
                for (int y = 0; y < InputHeight; y += 2)
                {
                    for (int x = 0; x < InputWidth; x++)
                    {
                        
                    }
                }

                //left right lines and "width" of
                for (int x = 0; x < InputWidth; x += 2)
                {
                    for (int y = 0; y < InputHeight; y++)
                    {

                    }
                }
            }
            else //Boxing
            {
                //Divide the input matrix into boxes of 4 InputNeurons
                for (int i = 0; i < InputWidth; i++)
                {
                    for (int j = 0; j < InputHeight; j++)
                    {

                    }
                }

            }

            //TODO Change for InputNeurons to HiddenNeurons
            Patterns.ForEach(x =>
            {
                OutputNeuron outputNeuron = new OutputNeuron();

                foreach (InputNeuron inputNeuron in InputNeurons)
                {
                    WeightedLink weightedLink = new WeightedLink() { Neuron = inputNeuron };

                    outputNeuron.InputLayer.Add(weightedLink);

                    // Add to Genomes
                    Genomes.Add(weightedLink);
                }

                OutputNeurons.Add(x, outputNeuron);
            });
        }

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

        #region Properties

        public double Fitness { get; private set; }

		public List<WeightedLink> Genomes { get; set; }

		public InputNeuron[,] InputNeurons { get; set; }

		public Dictionary<string, OutputNeuron> OutputNeurons { get; set; }

        public enum HiddenLayerType
        {
            Lining,
            Boxing,
        }

        public HiddenLayerType HiddenLayerMode{get; private set;}

        public int InputWidth
        {
            get;
            private set;
        }

        public int InputHeight
        {
            get;
            private set;
        }

        public List<HiddenNeuron> HiddenNeurons { get; set; }

        public List<string> Patterns { get; set; }

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

				IEnumerable<double> values =
					OutputNeurons.Where(x => x.Key != image.RepresentingInformation).Select(x => x.Value.ActivationValue);

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

					IEnumerable<double> values =
						OutputNeurons.Where(x => x.Key != image.RepresentingInformation).Select(x => x.Value.ActivationValue);

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
			for (int i = 0; i < pixelValues.GetLength(0); i++)
			{
				for (int j = 0; j < pixelValues.GetLength(1); j++)
				{
					InputNeurons[i, j].Value = pixelValues[i, j];
				}
			}
		}

	}
}