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
using NumberRecognizer.App.Help;
using NumberRecognizer.Cloud.Contract.Data;
using NumberRecognizer.Labeling;

namespace NumberRecognizer.App.ViewModel
{
    public class CreateNetworkViewModel : ViewModelBase
    {
        /// <summary>
        /// The command create network
        /// </summary>
        private ICommand cmdCreateNetwork;

        /// <summary>
        /// The command delete network
        /// </summary>
        private ICommand cmdCancel;

        /// <summary>
        /// The canvas list
        /// </summary>
        private IList<InkCanvasRT> canvasList;

        /// <summary>
        /// The network name
        /// </summary>
        private string networkName;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedNetworksViewModel"/> class.
        /// </summary>
        public CreateNetworkViewModel()
        {
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
            CreateNetwork = new RelayCommand(() => App.Frame.Navigate(typeof(CreateNetworkPage)));
        }

        /// <summary>
        /// Initializes the name of the network.
        /// </summary>
        private async void InitializeNetworkName()
        {
            NetworkName = string.Format("{0}_{1}", await UserInformation.GetLastNameAsync(), DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
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

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the add network.
        /// </summary>
        /// <value>
        /// The add network.
        /// </value>
        public ICommand CreateNetwork
        {
            get
            {
                return cmdCreateNetwork;
            }
            set
            {
                cmdCreateNetwork = value;
                RaisePropertyChanged(() => CreateNetwork);
            }
        }

        /// <summary>
        /// Gets or sets the refresh networks.
        /// </summary>
        /// <value>
        /// The refresh networks.
        /// </value>
        public ICommand Cancel
        {
            get
            {
                return cmdCancel;
            }
            set
            {
                cmdCancel= value;
                RaisePropertyChanged(() => Cancel);
            }
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the NextButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private async Task<Dictionary<int, TrainingImage>> GetTrainingData(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (InkCanvasRT inkCanvas in canvasList)
            {
                var renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(inkCanvas);

                byte[] canvasRGBABytes = (await renderTargetBitmap.GetPixelsAsync()).ToArray();
                byte[] canvasBytes = await ImageHelperRT.GetByteArrayFromRGBAByteArrayAsync(canvasRGBABytes, inkCanvas.ForegroundColor);

                double canvasWidth = inkCanvas.ActualWidth;
                double canvasHeight = inkCanvas.ActualHeight;
                double[,] canvasPixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(canvasBytes, canvasWidth, canvasHeight);

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

                    byte[] orgRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(orgBytes, Colors.Black);
                    await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(orgRGBABytes, mbr.Width, mbr.Height, inkCanvas.Name + "_org");
                    connectedComponent.Pixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(orgBytes, mbr.Width, mbr.Height);

                    byte[] scaRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(scaBytes, Colors.Black);
                    scaRGBABytes = await ImageHelperRT.ScaleRGBAByteArrayAsync(scaRGBABytes, mbr.Size, mbr.Size, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    scaBytes = await ImageHelperRT.GetByteArrayFromRGBAByteArrayAsync(scaRGBABytes, inkCanvas.ForegroundColor);
                    await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight, inkCanvas.Name + "_sca");
                    connectedComponent.ScaledPixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(scaBytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                }
            }

            //TODO
            return null;
        }

    }
}
