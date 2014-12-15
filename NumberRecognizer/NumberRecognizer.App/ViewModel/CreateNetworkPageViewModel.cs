//-----------------------------------------------------------------------
// <copyright file="CreateNetworkPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Create Network Page ViewModel.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Windows.Input;
    using NumberRecognition.Labeling;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.App.View;
    using PropertyChanged;
    using Windows.System.UserProfile;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Shapes;
    using Mutzl.MvvmLight;
    using GalaSoft.MvvmLight;
    using System.Threading.Tasks;

    /// <summary>
    /// Create Network Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class CreateNetworkPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The training images.
        /// </summary>
        private ObservableCollection<TrainingImageRT> trainingImagesRT = new ObservableCollection<TrainingImageRT>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNetworkPageViewModel"/> class.
        /// </summary>
        public CreateNetworkPageViewModel()
        {
            this.InitializeProperties();
            this.InitializeCommands();
        }

        /// <summary>
        /// Gets the ink canvas collection.
        /// </summary>
        /// <value>
        /// The ink canvas collection.
        /// </value>
        public ObservableCollection<InkCanvasRT> InkCanvasRTCollection { get; private set; }

        /// <summary>
        /// Gets or sets the name of the network.
        /// </summary>
        /// <value>
        /// The name of the network.
        /// </value>
        public string NetworkName { get; set; }

        /// <summary>
        /// Gets the next command.
        /// </summary>
        /// <value>
        /// The next command.
        /// </value>
        public ICommand NextCommand { get; private set; }

        /// <summary>
        /// Gets or sets the labeling command.
        /// </summary>
        /// <value>
        /// The labeling command.
        /// </value>
        public ICommand LabelingCommand { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is next command not executable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is next command not executable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNextCommandNotExecutable
        {
            get
            {
                return !this.CanExecuteNextCommand();
            }
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.LabelingCommand = new DependentRelayCommand(this.LabelingAsync, () => true == true, this, () => this);
            this.NextCommand = new DependentRelayCommand(this.NextPage, this.CanExecuteNextCommand, this, () => this);
        }

        /// <summary>
        /// Determines whether this instance [can execute next command].
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteNextCommand()
        {
            ////int networkNameLength = this.NetworkName.Length;
            ////int inkCanvasCollectonsWithComponents = this.InkCanvasRTCollection.Count(p => p.Labeling.ComponentCount > 0);
            ////return (networkNameLength > 0 && inkCanvasCollectonsWithComponents == 10);
            return true;
        }

        /// <summary>
        /// Labelings the asynchronous.
        /// </summary>
        private async void LabelingAsync()
        {
            trainingImagesRT = new ObservableCollection<TrainingImageRT>();
            foreach (InkCanvasRT inkCanvas in this.InkCanvasRTCollection)
            {
                inkCanvas.RefreshCanvas();
                var renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(inkCanvas);

                byte[] canvasRGBABytes = (await renderTargetBitmap.GetPixelsAsync()).ToArray();
                byte[] canvasBytes = ImageHelperRT.GetByteArrayFromRGBAByteArray(canvasRGBABytes, inkCanvas.ForegroundColor, inkCanvas.BackgroundColor);

                double canvasWidth = inkCanvas.ActualWidth;
                double canvasHeight = inkCanvas.ActualHeight;
                double[,] canvasPixels = ImageHelperRT.Get2DPixelArrayFromByteArray(canvasBytes, canvasWidth, canvasHeight);

                inkCanvas.Labeling = new ConnectedComponentLabeling();
                inkCanvas.Labeling.TwoPassLabeling(canvasPixels, 0.0);

                foreach (ConnectedComponent connectedComponent in inkCanvas.Labeling.ConnectedComponents)
                {
                    MinimumBoundingRectangle mbr = connectedComponent.MinBoundingRect;
                    Rectangle rectangle = new Rectangle()
                    {
                        Stroke = new SolidColorBrush(Colors.OrangeRed),
                        StrokeThickness = 2.0,
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
                            if (connectedComponent.ComponentPoints.Exists(p => p.X == mbr.Left + x && p.Y == mbr.Top + y))
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
                    connectedComponent.Pixels = ImageHelperRT.Get2DPixelArrayFromByteArray(orgBytes, mbr.Width, mbr.Height);

                    byte[] scaRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(scaBytes, Colors.Black);
                    scaRGBABytes = await ImageHelperRT.ScaleRGBAByteArrayAsync(scaRGBABytes, mbr.Size, mbr.Size, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    scaBytes = ImageHelperRT.GetByteArrayFromRGBAByteArray(scaRGBABytes, inkCanvas.ForegroundColor, inkCanvas.BackgroundColor);
                    await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight, inkCanvas.Name + "_sca");
                    connectedComponent.ScaledPixels = ImageHelperRT.Get2DPixelArrayFromByteArray(scaBytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);

                    TrainingImage trainingImage = new TrainingImage()
                    {
                        Height = (int)ImageHelperRT.ImageHeight,
                        Width = (int)ImageHelperRT.ImageWidth,
                        Pattern = inkCanvas.Name,
                        ImageData = ImageHelperRT.GetObservableCollectionFrom2DPixelArray(connectedComponent.ScaledPixels)
                    };
                    TrainingImageRT trainingImageRT = new TrainingImageRT(trainingImage);
                    var memoryStream = await ImageHelperRT.SaveRGBAByteArrayAsMemoryStream(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    trainingImageRT.Bitmap = new WriteableBitmap((int)ImageHelperRT.ImageWidth, (int)ImageHelperRT.ImageHeight);
                    await trainingImageRT.Bitmap.SetSourceAsync(memoryStream);

                    trainingImagesRT.Add(trainingImageRT);
                }
            }
        }

        /// <summary>
        /// Nexts the page.
        /// </summary>
        private void NextPage()
        {
            KeyValuePair<string, ObservableCollection<TrainingImageRT>> keyValuePair = new KeyValuePair<string, ObservableCollection<TrainingImageRT>>(this.NetworkName, trainingImagesRT);
            App.RootFrame.Navigate(typeof(GroupedImagesPage), keyValuePair);
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private async void InitializeProperties()
        {
            this.NetworkName = string.Format("{0}_{1}", await UserInformation.GetLastNameAsync(), DateTime.Now.ToFileTime());
            this.InkCanvasRTCollection = new ObservableCollection<InkCanvasRT>();
            for (int i = 0; i < 10; i++)
            {
                this.InkCanvasRTCollection.Add(new InkCanvasRT() { Name = i.ToString() });
            }
        }
    }
}
