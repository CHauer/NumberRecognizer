using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NumberRecognizer.App.Control;
using NumberRecognizer.App.DataModel;
using NumberRecognizer.App.Help;
using NumberRecognizer.App.View;
using NumberRecognizer.Cloud.Contract.Data;
using NumberRecognizer.Labeling;

namespace NumberRecognizer.App.ViewModel
{
    public class CreateNetworkViewModel : ViewModelBase
    {
        /// <summary>
        /// The command create network
        /// </summary>
        private ICommand cmdValidateTrainingData;

        /// <summary>
        /// The command delete network
        /// </summary>
        private RelayCommand<string> cmdClearInk;


        /// <summary>
        /// The canvas list
        /// </summary>
        private IList<InkCanvasRT> canvasList;

        /// <summary>
        /// The network name
        /// </summary>
        private string networkName;

        /// <summary>
        /// The show error
        /// </summary>
        private bool isValidationError;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedNetworksViewModel"/> class.
        /// </summary>
        public CreateNetworkViewModel()
        {
            IsValidationError = false;
            canvasList = new List<InkCanvasRT>();
            InitializeNetworkName();
            LoadCommands();
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads the commands.
        /// </summary>
        private void LoadCommands()
        {
            ValidateTrainingData = new RelayCommand(ExecuteValidateTrainingData);
            ClearInk = new RelayCommand<string>((string inkCanvasIndex) => canvasList[Convert.ToInt32(inkCanvasIndex)].ClearInk());
        }

        /// <summary>
        /// Initializes the name of the network.
        /// </summary>
        private async void InitializeNetworkName()
        {
            NetworkName = string.Format("{0}_{1}", await UserInformation.GetLastNameAsync(), DateTime.Now.ToFileTime());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the canvas list.
        /// </summary>
        /// <value>
        /// The canvas list.
        /// </value>
        public IList<InkCanvasRT> CanvasList
        {
            get
            {
                return canvasList;
            }
            private set
            {
                canvasList = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the network.
        /// </summary>
        /// <value>
        /// The name of the network.
        /// </value>
        public string NetworkName
        {
            get
            {
                return networkName;
            }
            set
            {
                networkName = value;
                RaisePropertyChanged(() => NetworkName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is validation error].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is validation error]; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationError
        {
            get
            {
                return isValidationError;
            }
            set
            {
                isValidationError = value;
                RaisePropertyChanged(() => IsValidationError);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the add network.
        /// </summary>
        /// <value>
        /// The add network.
        /// </value>
        public ICommand ValidateTrainingData
        {
            get
            {
                return cmdValidateTrainingData;
            }
            set
            {
                cmdValidateTrainingData = value;
                RaisePropertyChanged(() => ValidateTrainingData);
            }
        }

        /// <summary>
        /// Gets or sets the refresh networks.
        /// </summary>
        /// <value>
        /// The refresh networks.
        /// </value>
        public RelayCommand<string> ClearInk
        {
            get
            {
                return cmdClearInk;
            }
            set
            {
                cmdClearInk = value;
                RaisePropertyChanged(() => ClearInk);
            }
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the NextButton control.
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<int, List<LocalTrainingImage>>> GetTrainingData()
        {
            Dictionary<int, List<LocalTrainingImage>> trainingData = new Dictionary<int,List<LocalTrainingImage>>();
            int pattern = 0; 

            foreach (InkCanvasRT inkCanvas in canvasList)
            {
                var renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(inkCanvas);

                byte[] canvasRGBABytes = (await renderTargetBitmap.GetPixelsAsync()).ToArray();
                byte[] canvasBytes = await ImageHelperRT.GetByteArrayFromRGBAByteArrayAsync(canvasRGBABytes, inkCanvas.ForegroundColor);

                double canvasWidth = inkCanvas.ActualWidth;
                double canvasHeight = inkCanvas.ActualHeight;
                double[,] canvasPixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(canvasBytes, canvasWidth, canvasHeight);

                trainingData.Add(pattern, new List<LocalTrainingImage>());

                inkCanvas.ConnectedComponentLabeling = new ConnectedComponentLabeling(canvasPixels, 0.0);

                foreach (ConnectedComponent connectedComponent in inkCanvas.ConnectedComponentLabeling.ConnectedComponents)
                {
                    MinimumBoundingRectangle mbr = connectedComponent.MinBoundingRect;
                    Rectangle rectangle = new Rectangle()
                    {
                        Stroke = new SolidColorBrush(Colors.Blue),
                        StrokeThickness = inkCanvas.StrokeThickness / 2,
                        Width = mbr.Width,
                        Height = mbr.Height,
                        Margin = new Thickness() { Top = mbr.Top, Left = mbr.Left }
                    };
                    inkCanvas.Children.Add(rectangle);

                    byte[] orgBytes = new byte[(int)(mbr.Width * mbr.Height)];
                    byte[] scaBytes = new byte[(int)Math.Pow(mbr.Size, 2)];
                    for (int y = 0; y < mbr.Height; y++)
                    {
                        for (int x = 0; x < mbr.Width; x++)
                        {
                            if (connectedComponent.Points.Exists(p => p.X == mbr.Left + x && p.Y == mbr.Top + y))
                            {
                                double row = (mbr.Top + y) * canvasWidth;
                                double col = mbr.Left + x;
                                byte val = canvasBytes[(int)(row + col)];

                                int orgIdx = (int)(y * mbr.Width) + x;
                                orgBytes[orgIdx] = val;

                                int scaIdx = (int)(((y + mbr.PadTop) * mbr.Size) + (x + mbr.PadLeft));
                                scaBytes[scaIdx] = val;
                            }
                        }
                    }

                    //byte[] orgRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(orgBytes, Colors.Black);
                    //await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(orgRGBABytes, mbr.Width, mbr.Height, inkCanvas.Name + "_org");
                    //connectedComponent.Pixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(orgBytes, mbr.Width, mbr.Height);

                    byte[] scaRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(scaBytes, Colors.Black);
                    scaRGBABytes = await ImageHelperRT.ScaleRGBAByteArrayAsync(scaRGBABytes, mbr.Size, mbr.Size, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    scaBytes = await ImageHelperRT.GetByteArrayFromRGBAByteArrayAsync(scaRGBABytes, inkCanvas.ForegroundColor);
                    double[,] pixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(scaBytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    connectedComponent.ScaledPixels = pixels;

                    //save image to storage
                    string imagename = string.Format("{0}_{1}", networkName, pattern);
                    string path = await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight, imagename);

                    var imageData = new TrainingImage(){
                                    Height = Convert.ToInt32(ImageHelperRT.ImageHeight),
                                    Width = Convert.ToInt32(ImageHelperRT.ImageWidth),
                                    Pattern = pattern.ToString()
                    };
                            
                    imageData.TransformFrom2DArrayToImageData(pixels);

                    trainingData[pattern].Add(new LocalTrainingImage()
                    {
                        LocalImagePath = path,
                        ImageData = imageData
                    });    
                }

                pattern++;
            }

            return trainingData;
        
        }

        /// <summary>
        /// Executes the validate training data.
        /// </summary>
        private async void ExecuteValidateTrainingData()
        {
            var trainData = await GetTrainingData();

            IsValidationError = !trainData.All(keyvalpair => keyvalpair.Value.Count > 0);
            
            if(!IsValidationError)
            {
                App.Frame.Navigate(typeof(ValidateTrainingDataPage), trainData);
            }
        }
    }
}
