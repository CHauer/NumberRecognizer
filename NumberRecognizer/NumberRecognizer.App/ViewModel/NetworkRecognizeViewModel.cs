using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using NumberRecognizer.Cloud.Contract.Data;
using PropertyChanged;
using NumberRecognition.Labeling;
using NumberRecognizer.App.Common;
using NumberRecognizer.App.Control;
using NumberRecognizer.App.DataModel;
using NumberRecognizer.App.Help;
using NumberRecognizer.App.NumberRecognizerService;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace NumberRecognizer.App.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    [ImplementPropertyChanged]
    public class NetworkRecognizeViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkRecognizeViewModel"/> class.
        /// </summary>
        public NetworkRecognizeViewModel(NetworkInfo networkInfo)
        {
            this.Network = networkInfo;
            this.InitializeCommands();
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.RecognizeNumber = new RelayCommand(this.ExecuteRecognizeNumber);
            this.ClearResult = new RelayCommand(this.ClearPage);
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
        /// Executes the recognize number.
        /// </summary>
        private async void ExecuteRecognizeNumber()
        {
            if (InkCanvas == null)
            {
                return;
            }

            RecognitionImages = new ObservableCollection<RecognitionImage>();

            await LabelingHelperRT.ConnectedComponentLabelingForInkCanvasRT(InkCanvas);
            foreach (ConnectedComponent component in InkCanvas.Labeling.ConnectedComponents)
            {
                RecognitionImage recognitionImage = new RecognitionImage
                {
                    Height = (int)ImageHelperRT.ImageHeight,
                    Width = (int)ImageHelperRT.ImageWidth
                };
                recognitionImage.TransformFrom2DArrayToImageData(component.ScaledPixels);

                this.RecognitionImages.Add(recognitionImage);
            }

            try
            {
                NumberRecognizerServiceClient serviceProxy = new NumberRecognizerServiceClient();
                Result = await serviceProxy.RecognizePhoneNumberAsync(Network.NetworkId, RecognitionImages);

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
            ChartResult = new ObservableCollection<object>();

            foreach (NumberRecognitionResultItem item in Result.Items)
            {
                var numberPropabilities = new ObservableCollection<ChartPopulation>();
                foreach (KeyValuePair<char, double> pair in item.Probabilities)
                {
                    numberPropabilities.Add(new ChartPopulation(){
                        Name = pair.Key.ToString(),
                        Value = pair.Value < 0 ? 0 : pair.Value * 100,
                    });
                }
                ChartResult.Add(new
                {
                    Number = item.NumericCharacter.ToString(),
                    Values = numberPropabilities
                });
            }

            RaisePropertyChanged(() => ChartResult); 
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
