//-----------------------------------------------------------------------
// <copyright file="NetworkRecognizeViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Recognize ViewModel.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using Mutzl.MvvmLight;
    using NumberRecognition.Labeling;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;
    using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

    /// <summary>
    /// Network Recognize ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class NetworkRecognizeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkRecognizeViewModel"/> class.
        /// </summary>
        /// <param name="networkInfo">The network information.</param>
        public NetworkRecognizeViewModel(NetworkInfo networkInfo)
        {
            this.Network = networkInfo;
            this.InitializeCommands();
        }

        /// <summary>
        /// Gets or sets the network.
        /// </summary>
        /// <value>
        /// The network.
        /// </value>
        public NetworkInfo Network { get; set; }

        /// <summary>
        /// Gets or sets the network identifier.
        /// </summary>
        /// <value>
        /// The network identifier.
        /// </value>
        public InkCanvasRT InkCanvas { get; set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public NumberRecognitionResult Result { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets the recognize number.
        /// </summary>
        /// <value>
        /// The recognize number.
        /// </value>
        public ICommand RecognizeNumber { get; set; }

        /// <summary>
        /// Gets or sets the reset ink canvas command.
        /// </summary>
        /// <value>
        /// The reset ink canvas command.
        /// </value>
        public ICommand ResetInkCanvasCommand { get; set; }

        /// <summary>
        /// Gets or sets the clear result.
        /// </summary>
        /// <value>
        /// The clear result.
        /// </value>
        public ICommand ClearResult { get; set; }

        /// <summary>
        /// Gets or sets the chart result.
        /// </summary>
        /// <value>
        /// The chart result.
        /// </value>
        public ObservableCollection<object> ChartResult { get; set; }

        /// <summary>
        /// Gets or sets the recognition images.
        /// </summary>
        /// <value>
        /// The recognition images.
        /// </value>
        public ObservableCollection<RecognitionImage> RecognitionImages { get; set; }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.RecognizeNumber = new DependentRelayCommand(this.ExecuteRecognizeNumber, () => this.IsLoading == false, this, () => this.IsLoading);
            this.ClearResult = new RelayCommand(this.ClearPage);
            this.ResetInkCanvasCommand = new RelayCommand(this.ResetInkCanvas);
        }

        /// <summary>
        /// Resets the ink canvas.
        /// </summary>
        private void ResetInkCanvas()
        {
            if (this.InkCanvas != null)
            {
                this.InkCanvas.ClearInk();
            }
        }

        /// <summary>
        /// Executes the recognize number.
        /// </summary>
        private async void ExecuteRecognizeNumber()
        {
            if (this.InkCanvas == null)
            {
                return;
            }
            else
            {
                this.IsLoading = true;
                this.RecognitionImages = new ObservableCollection<RecognitionImage>();

                await LabelingHelperRT.ConnectedComponentLabelingForInkCanvasRT(this.InkCanvas);
                foreach (ConnectedComponent component in this.InkCanvas.Labeling.ConnectedComponents.OrderBy(p => p.MinBoundingRect.Left).ToList())
                {
                    try
                    {
                        RecognitionImage recognitionImage = new RecognitionImage
                        {
                            Height = (int)ImageHelperRT.ImageHeight,
                            Width = (int)ImageHelperRT.ImageWidth
                        };
                        recognitionImage.TransformFrom2DArrayToImageData(component.ScaledPixels);

                        this.RecognitionImages.Add(recognitionImage);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                try
                {
                    NumberRecognizerServiceClient serviceProxy = new NumberRecognizerServiceClient();
                    this.Result = await serviceProxy.RecognizePhoneNumberAsync(this.Network.NetworkId, this.RecognitionImages);

                    this.CreateChartData();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                this.IsLoading = false;
            }
        }

        /// <summary>
        /// Creates the chart data.
        /// </summary>
        private void CreateChartData()
        {
            this.ChartResult = new ObservableCollection<object>();

            foreach (NumberRecognitionResultItem item in this.Result.Items)
            {
                var numberPropabilities = new ObservableCollection<ChartPopulation>();
                foreach (KeyValuePair<char, double> pair in item.Probabilities)
                {
                    numberPropabilities.Add(new ChartPopulation()
                    {
                        Name = pair.Key.ToString(),
                        Value = pair.Value < 0 ? 0 : pair.Value * 100,
                    });
                }

                this.ChartResult.Add(new
                {
                    Number = item.NumericCharacter.ToString(),
                    Values = numberPropabilities
                });
            }

            this.RaisePropertyChanged(() => this.ChartResult);
        }

        /// <summary>
        /// Clears the page.
        /// </summary>
        private void ClearPage()
        {
            this.InkCanvas.ClearInk();
            this.Result = null;
            this.ChartResult = null;
        }
    }
}
