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
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using Mutzl.MvvmLight;
    using NumberRecognition.Labeling;
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
        /// <returns>Can execute next command.</returns>
        private bool CanExecuteNextCommand()
        {
            ////int networkNameLength = this.NetworkName.Length;
            ////int inkCanvasCollectonsWithComponents = this.InkCanvasRTCollection.Count(p => p.Labeling.ComponentCount > 0);
            ////return (networkNameLength > 0 && inkCanvasCollectonsWithComponents == 10);
            return true;
        }

        /// <summary>
        /// Labeling asynchronous.
        /// </summary>
        private async void LabelingAsync()
        {
            this.trainingImagesRT = new ObservableCollection<TrainingImageRT>();

            foreach (InkCanvasRT inkCanvas in this.InkCanvasRTCollection)
            {
                await LabelingHelperRT.ConnectedComponentLabelingForInkCanvasRT(inkCanvas);
                foreach (ConnectedComponent component in inkCanvas.Labeling.ConnectedComponents)
                {
                    TrainingImage trainingImage = new TrainingImage();
                    trainingImage.Height = (int)component.MinBoundingRect.Size;
                    trainingImage.Width = (int)component.MinBoundingRect.Size;
                    trainingImage.Pattern = inkCanvas.Name;
                    trainingImage.ImageData = ImageHelperRT.GetObservableCollectionFrom2DPixelArray(component.ScaledPixels);

                    TrainingImageRT trainingImageRT = new TrainingImageRT(trainingImage);
                    var scaRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(component.ScaledBytes, inkCanvas.ForegroundColor);
                    var memoryStream = await ImageHelperRT.SaveRGBAByteArrayAsMemoryStream(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    trainingImageRT.Bitmap = new WriteableBitmap((int)ImageHelperRT.ImageWidth, (int)ImageHelperRT.ImageHeight);
                    await trainingImageRT.Bitmap.SetSourceAsync(memoryStream);

                    this.trainingImagesRT.Add(trainingImageRT);
                }
            }
        }

        /// <summary>
        /// Next  page.
        /// </summary>
        private void NextPage()
        {
            KeyValuePair<string, ObservableCollection<TrainingImageRT>> keyValuePair = new KeyValuePair<string, ObservableCollection<TrainingImageRT>>(this.NetworkName, this.trainingImagesRT);
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
