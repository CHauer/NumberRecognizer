//-----------------------------------------------------------------------
// <copyright file="NetworkDetailPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Detail Page ViewModel.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.ViewModel
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.NumberRecognizerService;
    using PropertyChanged;
    using De.TorstenMandelkow.MetroChart;
    using System.Collections.ObjectModel;

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
        /// Gets the calculate network command.
        /// </summary>
        /// <value>
        /// The calculate network command.
        /// </value>
        public ICommand CalculateNetworkCommand { get; private set; }

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
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void InitializeProperties()
        {
            this.Network = new NetworkInfo();
        }
    }
}
