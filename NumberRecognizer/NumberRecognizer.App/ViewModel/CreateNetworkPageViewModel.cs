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
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Mutzl.MvvmLight;
    using NumberRecognition.Labeling;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.View;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;
    using Windows.System.UserProfile;
    using Windows.UI.Xaml.Media.Imaging;

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
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.LabelingCommand = new DependentRelayCommand(this.LabelingAsync, () => this.IsLoading == false, this, () => this.IsLoading);
            this.NextCommand = new DependentRelayCommand(this.NextPage, this.CanExecuteNextCommand, this, () => this.NetworkName, () => this.InkCanvasRTCollection, () => this.IsShowHint);
            this.ClearCanvas = new RelayCommand<InkCanvasRT>((canvas) => canvas.ClearInk());
        }

        /// <summary>
        /// Determines whether this instance [can execute next command].
        /// </summary>
        /// <returns>Can execute next command.</returns>
        private bool CanExecuteNextCommand()
        {
            if (string.IsNullOrEmpty(this.NetworkName))
            {
                return false;
            }

            if (!this.InkCanvasRTCollection.All(p => p.Labeling.ComponentCount > 0))
            {
                this.IsShowHint = true;
                return false;
            }

            this.IsShowHint = false;
            return true;
        }

        /// <summary>
        /// Labeling asynchronous.
        /// </summary>
        private async void LabelingAsync()
        {
            this.IsLoading = true;
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

            this.NextCommand.RaiseCanExecuteChanged();
            this.RaisePropertyChanged(() => this.IsShowHint);
            this.RaisePropertyChanged(() => this.InkCanvasRTCollection);
            this.RaisePropertyChanged(() => this.NetworkName);
            this.IsLoading = false;
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
