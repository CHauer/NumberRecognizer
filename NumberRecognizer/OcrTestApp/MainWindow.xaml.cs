using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using NumberRecognizer.Lib.Network;
using NumberRecognizer.Lib.Training;

namespace OcrTestApp
{
	// TODO: Refactor
	public partial class MainWindow : Window
	{
		private const int ImageHeight = 16;

		private const int ImageWidth = 16;

		private const string TrainingDataPath = @".\TrainingData";

        private NetworkTrainer trainer;

		public MainWindow()
		{
			InitializeComponent();
		}

		private PatternRecognitionNetwork ResultNetwork { get; set; }

        

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			this.DrawCanvas.Strokes.Clear();
		}

		private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
		{
			// Confirm parent and childName are valid.
			if (parent == null)
			{
				return null;
			}

			T foundChild = null;

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);

				// If the child is not of the request child type child
				T childType = child as T;
				if (childType == null)
				{
					// recursively drill down the tree
					foundChild = FindChild<T>(child, childName);

					// If the child is found, break so we do not overwrite the found child.
					if (foundChild != null)
					{
						break;
					}
				}
				else if (!string.IsNullOrEmpty(childName))
				{
					FrameworkElement frameworkElement = child as FrameworkElement;

					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName)
					{
						// if the child's name is of the request name
						foundChild = (T)child;
						break;
					}
				}
				else
				{
					// child element found.
					foundChild = (T)child;
					break;
				}
			}

			return foundChild;
		}

		private void RecognizeButton_Click(object sender, RoutedEventArgs e)
		{
			double[,] pixelsFromCanvas = ImageHelper.GetPixelsFromCanvas(this.DrawCanvas, ImageWidth, ImageHeight);

			List<RecognitionResult> recognizeCharacters =
				ResultNetwork.RecognizeCharacter(pixelsFromCanvas).OrderByDescending(x => x.Propability).ToList();

			this.RecognizedFirstPatternLabel.Content = recognizeCharacters[0].RecognizedCharacter;
			this.RecognizedFirstPropabilityLabel.Content = recognizeCharacters[0].Propability.ToString("F8");

			this.RecognizedSecondPatternLabel.Content = recognizeCharacters[1].RecognizedCharacter;
			this.RecognizedSecondPropabilityLabel.Content = recognizeCharacters[1].Propability.ToString("F8");

			this.RecognizedThirdPatternLabel.Content = recognizeCharacters[2].RecognizedCharacter;
			this.RecognizedThirdPropabilityLabel.Content = recognizeCharacters[2].Propability.ToString("F8");
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			int index = 0;

			string path = Path.Combine(TrainingDataPath, (string)this.SaveAsComboBox.SelectedValue,
				string.Format("{0}{1}.png", this.SaveAsComboBox.SelectedValue, index));

			while (File.Exists(path))
			{
				index++;
				path = Path.Combine(TrainingDataPath, (string)this.SaveAsComboBox.SelectedValue,
					string.Format("{0}{1}.png", this.SaveAsComboBox.SelectedValue, index));
			}

			ImageHelper.SaveInkCanvasToPng(this.DrawCanvas, path, ImageWidth, ImageHeight);
		}

		private IEnumerable<PatternRecognitionNetwork> TrainNetwork()
		{
            trainer = new NetworkTrainer(ImageHelper.ReadTrainingData(TrainingDataPath));

            trainer.GenerationChanged += NetworkTrainer_HandleGenerationChanged;
            
            return trainer.TrainNetwork();

            #region Not Used
            //// Parameters
            //const int populationSize = 100;
            //const int maxGenerations = 100;

            //const double truncationSelectionPercentage = 0.1;
            //const int truncationSelectionAbsolute = (int)(populationSize * truncationSelectionPercentage);

            //// Read Imagedata
            //TrainingData = new ConcurrentBag<TrainingImage>(ImageHelper.ReadTrainingData(TrainingDataPath));

            //// Create initial population
            //ConcurrentBag<PatternRecognitionNetwork> currentGeneration =
            //    new ConcurrentBag<PatternRecognitionNetwork>(new List<PatternRecognitionNetwork>());

            //if (currentGeneration.Count == 0)
            //{
            //    Parallel.For(0, populationSize, i =>
            //    {
            //        ThreadSafeRandom random = new ThreadSafeRandom();

            //        PatternRecognitionNetwork patternRecognitionNetwork = CreateNetwork();
            //        patternRecognitionNetwork.Genomes.ForEach(x => x.Weight = (2 * random.NextDouble()) - 1);

            //        currentGeneration.Add(patternRecognitionNetwork);
            //    });
            //}

            //List<PatternRecognitionNetwork> networks = currentGeneration.ToList();

            //for (int i = 0; i < maxGenerations; i++)
            //{
            //    // Calculate Fitness
            //    Parallel.ForEach(currentGeneration, individual => individual.CalculateFitness(TrainingData.ToList()));

            //    PatternRecognitionNetwork bestNetwork = currentGeneration.OrderByDescending(x => x.Fitness).First();

            //    

            //    ConcurrentBag<PatternRecognitionNetwork> newGeneration = new ConcurrentBag<PatternRecognitionNetwork>();

            //    // Specify individuals for recombination (truncation selection)
            //    IEnumerable<PatternRecognitionNetwork> patternRecognitionNetworks =
            //        currentGeneration.OrderByDescending(x => x.Fitness).Take(truncationSelectionAbsolute);

            //    // TODO: Add recombination / crossover
            //    Parallel.For(0, populationSize, x =>
            //    {
            //        ThreadSafeRandom random = new ThreadSafeRandom();

            //        PatternRecognitionNetwork patternRecognitionNetwork =
            //            patternRecognitionNetworks.ToList().OrderBy(individual => Guid.NewGuid()).First();

            //        // Create children
            //        PatternRecognitionNetwork firstChild = CreateNetwork();

            //        // Copy weights
            //        for (int j = 0; j < patternRecognitionNetwork.Genomes.Count; j++)
            //        {
            //            firstChild.Genomes[j].Weight = patternRecognitionNetwork.Genomes[j].Weight;
            //        }

            //        // Simple random mutation
            //        firstChild.Genomes.ForEach(genome =>
            //        {
            //            if (random.NextDouble() > 0.95)
            //            {
            //                genome.Weight += (random.NextDouble() * 2) - 1;
            //            }
            //        });

            //        // Add new children
            //        newGeneration.Add(firstChild);
            //    });

            //    currentGeneration = newGeneration;
            //}

            //// Calculate Fitness
            //Parallel.ForEach(currentGeneration, individual => individual.CalculateFitness(TrainingData.ToList()));

            //return currentGeneration.ToList();

            #endregion
        }

        private void NetworkTrainer_HandleGenerationChanged(int currentGeneration, PatternRecognitionNetwork bestNetwork)
        {
            // GUI Update
            Dictionary<string, double> fitnessDetail = bestNetwork.GetFitnessDetail(trainer.TrainingData.ToList());

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.CurrentGenerationLabel.Content = currentGeneration;
                this.CurrentFitnessLabel.Content = bestNetwork.Fitness.ToString("F8");

                for (int j = 0; j < fitnessDetail.Count; j++)
                {
                    Label patternLabel = FindChild<Label>(this, string.Format("CurrentPattern{0}", j));
                    patternLabel.Content = fitnessDetail.ToList()[j].Key;

                    Label patternScoreLabel = FindChild<Label>(this, string.Format("CurrentPatternScore{0}", j));
                    patternScoreLabel.Content = fitnessDetail.ToList()[j].Value.ToString("F8");
                }
            }));
        }

        private void TrainNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ResultNetwork = TrainNetwork().OrderBy(x => x.Fitness).First();

                Dictionary<string, double> fitnessDetail = ResultNetwork.GetFitnessDetail(trainer.TrainingData.ToList());

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    //this.CurrentGenerationLabel.Content = currentGeneration;
                    this.CurrentFitnessLabel.Content = ResultNetwork.Fitness.ToString("F8");

                    for (int j = 0; j < fitnessDetail.Count; j++)
                    {
                        Label patternLabel = FindChild<Label>(this, string.Format("CurrentPattern{0}", j));
                        patternLabel.Content = fitnessDetail.ToList()[j].Key;

                        Label patternScoreLabel = FindChild<Label>(this, string.Format("CurrentPatternScore{0}", j));
                        patternScoreLabel.Content = fitnessDetail.ToList()[j].Value.ToString("F8");
                    }
                }));

            });
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(TrainingDataPath);

			foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
			{
				this.SaveAsComboBox.Items.Add(directory.Name);
			}

			this.SaveAsComboBox.SelectedIndex = 0;
		}
	}
}