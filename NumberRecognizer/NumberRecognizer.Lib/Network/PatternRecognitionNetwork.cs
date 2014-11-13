namespace NumberRecognizer.Lib.Network
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class PatternRecognitionNetwork
	{
		public PatternRecognitionNetwork(int width, int height, IEnumerable<string> patterns)
		{
			Create(width, height, patterns);
		}

		public double Fitness { get; private set; }

		public List<WeightedLink> Genomes { get; set; }

		public InputNeuron[,] InputNeurons { get; set; }

		public Dictionary<string, OutputNeuron> OutputNeurons { get; set; }

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

		public void SetInputData(double[,] pixelValues)
		{
			for (int i = 0; i < pixelValues.GetLength(0); i++)
			{
				for (int j = 0; j < pixelValues.GetLength(1); j++)
				{
					InputNeurons[i, j].Value = pixelValues[i, j];
				}
			}
		}

		private void Create(int width, int height, IEnumerable<string> patterns)
		{
			// Initialize Genomes
			Genomes = new List<WeightedLink>();

			// Input Neurons
			InputNeurons = new InputNeuron[width, height];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					InputNeurons[i, j] = new InputNeuron();
				}
			}

			// TODO: Add middle layer

			// Create Output Neurons
			OutputNeurons = new Dictionary<string, OutputNeuron>();

			patterns.ToList().ForEach(x =>
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
	}
}