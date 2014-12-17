//-----------------------------------------------------------------------
// <copyright file="CreateNetworkPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Create Network Page ViewModel.</summary>
//-----------------------------------------------------------------------


using System.Diagnostics;

namespace NumberRecognizer.App.ViewModel
{
    using GalaSoft.MvvmLight;
    using Mutzl.MvvmLight;
    using NumberRecognition.Labeling;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.App.View;
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Windows.Input;
    using Windows.System.UserProfile;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Shapes;
    using System.Linq;
    using GalaSoft.MvvmLight.Command;
    using NumberRecognizer.Cloud.Contract.Data;

    /// <summary>
    /// Create Network Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class CreateNetworkPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The training images.
        /// </summary>
        private ObservableCollection<LocalTrainingImage> trainingImagesRT = new ObservableCollection<LocalTrainingImage>();

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
        public RelayCommand NextCommand { get; private set; }

        /// <summary>
        /// Gets or sets the labeling command.
        /// </summary>
        /// <value>
        /// The labeling command.
        /// </value>
        public ICommand LabelingCommand { get; set; }

        /// <summary>
        /// Gets or sets the clear canvas.
        /// </summary>
        /// <value>
        /// The clear canvas.
        /// </value>
        public ICommand ClearCanvas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is error].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is error]; otherwise, <c>false</c>.
        /// </value>
        public bool IsShowHint { get; set; }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.LabelingCommand = new RelayCommand(this.LabelingAsync);
            this.NextCommand = new RelayCommand(this.NextPage, this.CanExecuteNextCommand);
            this.ClearCanvas = new RelayCommand<InkCanvasRT>((canvas) => canvas.ClearInk());
        }

        /// <summary>
        /// Determines whether this instance [can execute next command].
        /// </summary>
        /// <returns>Can execute next command.</returns>
        private bool CanExecuteNextCommand()
        {
            if(String.IsNullOrEmpty(NetworkName))
            {
                return false;
            }

            if (!this.InkCanvasRTCollection.All(p => p.Labeling.ComponentCount > 0))
            {
                IsShowHint = true;
                return false;
            }

            IsShowHint = false;
            return true;
        }

        /// <summary>
        /// Labeling asynchronous.
        /// </summary>
        private async void LabelingAsync()
        {
            this.trainingImagesRT = new ObservableCollection<LocalTrainingImage>();

            foreach (InkCanvasRT inkCanvas in this.InkCanvasRTCollection)
            {
                
                await LabelingHelperRT.ConnectedComponentLabelingForInkCanvasRT(inkCanvas);
                foreach (ConnectedComponent component in inkCanvas.Labeling.ConnectedComponents)
                {
                    try
                    {
                        TrainingImage trainingImage = new TrainingImage();
                        trainingImage.Height = (int)ImageHelperRT.ImageHeight;
                        trainingImage.Width = (int)ImageHelperRT.ImageWidth;
                        trainingImage.Pattern = inkCanvas.Name;
                        trainingImage.TransformFrom2DArrayToImageData(component.ScaledPixels);

                        LocalTrainingImage localTrainingImage = new LocalTrainingImage(trainingImage);
                        var scaRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(component.ScaledBytes, inkCanvas.ForegroundColor);
                        var memoryStream = await ImageHelperRT.SaveRGBAByteArrayAsMemoryStream(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                        localTrainingImage.Bitmap = new WriteableBitmap((int)ImageHelperRT.ImageWidth, (int)ImageHelperRT.ImageHeight);
                        await localTrainingImage.Bitmap.SetSourceAsync(memoryStream);

                        this.trainingImagesRT.Add(localTrainingImage);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            NextCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => IsShowHint);
        }

        /// <summary>
        /// Next  page.
        /// </summary>
        private void NextPage()
        {
            KeyValuePair<string, ObservableCollection<LocalTrainingImage>> keyValuePair = new KeyValuePair<string, ObservableCollection<LocalTrainingImage>>(this.NetworkName, this.trainingImagesRT);
            App.RootFrame.Navigate(typeof(GroupedImagesPage), keyValuePair);
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private async void InitializeProperties()
        {
            this.IsShowHint = false;
            this.NetworkName = string.Format("{0}_{1}", await UserInformation.GetLastNameAsync(), DateTime.Now.ToFileTime());
            this.InkCanvasRTCollection = new ObservableCollection<InkCanvasRT>();
            for (int i = 0; i < 10; i++)
            {
                this.InkCanvasRTCollection.Add(new InkCanvasRT() { Name = i.ToString() });
            }
        }
    }
}
