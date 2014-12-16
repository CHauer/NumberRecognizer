//-----------------------------------------------------------------------
// <copyright file="NetworkDetailPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Detail Page ViewModel.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.App.ViewModel
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;
    using System.Collections.ObjectModel;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.Control;
    using NumberRecognition.Labeling;

    /// <summary>
    /// Network Detail Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class NetworkDetailPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDetailPageViewModel"/> class.
        /// </summary>
        public NetworkDetailPageViewModel()
        {
            this.InitializeProperties();
            this.InitializeCommands();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDetailPageViewModel"/> class.
        /// </summary>
        /// <param name="network">The network.</param>
        public NetworkDetailPageViewModel(NetworkInfo network)
            : this()
        {
            this.Network = network;
        }

        /// <summary>
        /// Gets or sets the network.
        /// </summary>
        /// <value>
        /// The network.
        /// </value>
        public NetworkInfo Network { get; set; }


        /// <summary>
        /// Gets or sets the ink canvas.
        /// </summary>
        /// <value>
        /// The ink canvas.
        /// </value>
        public InkCanvasRT InkCanvas { get; set; }

        /// <summary>
        /// Gets or sets the recognize number command.
        /// </summary>
        /// <value>
        /// The recognize number command.
        /// </value>
        public ICommand RecognizeNumberCommand { get; set; }

        /// <summary>
        /// Gets or sets the recognized number.
        /// </summary>
        /// <value>
        /// The recognized number.
        /// </value>
        public string RecognizedNumber { get; set; }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.RecognizeNumberCommand = new RelayCommand(this.RecognizePhoneNumber);
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void InitializeProperties()
        {
            this.Network = new NetworkInfo();
        }

        /// <summary>
        /// Recognizes the number.
        /// </summary>
        private async void RecognizePhoneNumber()
        {
            ObservableCollection<RecognitionImage> recognitionImages = new ObservableCollection<RecognitionImage>();

            await LabelingHelperRT.ConnectedComponentLabelingForInkCanvasRT(this.InkCanvas);
            foreach (ConnectedComponent component in InkCanvas.Labeling.ConnectedComponents.OrderBy(p => p.MinBoundingRect.Left))
            {
                try
                {
                    RecognitionImage recognitionImage = new RecognitionImage()
                    {
                        Width = (int)component.MinBoundingRect.Size,
                        Height = (int)component.MinBoundingRect.Size,
                    };

                    recognitionImage.TransformFrom2DArrayToImageData(component.ScaledPixels);
                    recognitionImages.Add(recognitionImage);
                }
                catch
                {
                }
            }

            NumberRecognizerServiceClient serviceClient = new NumberRecognizerServiceClient();
            NumberRecognitionResult numberRecognitionResult = await serviceClient.RecognizePhoneNumberAsync(this.Network.NetworkId, recognitionImages);
            this.RecognizedNumber = numberRecognitionResult.Number;
        }
    }
}
