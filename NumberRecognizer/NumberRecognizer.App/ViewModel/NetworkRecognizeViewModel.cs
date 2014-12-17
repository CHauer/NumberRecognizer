//-----------------------------------------------------------------------
// <copyright file="NetworkRecognizeViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Detail Page ViewModel.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.App.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using NumberRecognition.Labeling;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;
   
    using Windows.UI.Xaml.Media.Imaging;
    using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

    /// <summary>
    /// NetworkRecognize ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class NetworkRecognizeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkRecognizeViewModel" /> class.
        /// </summary>
        /// <param name="networkInfo">The network information.</param>
        public NetworkRecognizeViewModel(NetworkInfo networkInfo)
        {
            this.Network = networkInfo;
            this.InitializeCommands();
        }

        /// <summary>
        /// Gets or sets the network identifier.
        /// </summary>
        /// <value>
        /// The network identifier.
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
            this.RecognizeNumber = new RelayCommand(this.ExecuteRecognizeNumber);
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
        }

        /// <summary>
        /// Creates the chart data.
        /// </summary>
        private void CreateChartData()
        {
            this.ChartResult = new ObservableCollection<object>();

            foreach (NumberRecognitionResultItem item in this.Result.Items)
            {
                var posNumberPropabilities = new ObservableCollection<ChartPopulation>();
                var negNumberPropabilities = new ObservableCollection<ChartPopulation>();

                foreach (KeyValuePair<char, double> pair in item.Probabilities)
                {
                    if (pair.Value < 0)
                    {
                        negNumberPropabilities.Add(new ChartPopulation()
                        {
                            Name = pair.Key.ToString(),
                            Value = Math.Abs(pair.Value * 100),
                        });
                    }
                    else
                    {
                        posNumberPropabilities.Add(new ChartPopulation()
                        {
                            Name = pair.Key.ToString(),
                            Value = pair.Value * 100
                        });
                    }
                }

                this.ChartResult.Add(new
                {
                    Number = item.NumericCharacter.ToString(),
                    Values = posNumberPropabilities,
                    NegValues = negNumberPropabilities
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
