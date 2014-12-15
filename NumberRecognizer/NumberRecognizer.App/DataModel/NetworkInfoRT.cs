//-----------------------------------------------------------------------
// <copyright file="NetworkInfoRT.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Info RT.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.DataModel
{
    using System.Windows.Input;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.App.View;
    using PropertyChanged;

    /// <summary>
    /// Network Info RT.
    /// </summary>
    [ImplementPropertyChanged]
    public class NetworkInfoRT
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInfoRT"/> class.
        /// </summary>
        public NetworkInfoRT()
        {
            this.ClickNetworkInfoRTCommand = new RelayCommand(this.NetworkInfoRTClicked);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInfoRT"/> class.
        /// </summary>
        /// <param name="network">The network.</param>
        public NetworkInfoRT(NetworkInfo network)
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
        /// Gets the click network information command.
        /// </summary>
        /// <value>
        /// The click network information command.
        /// </value>
        public ICommand ClickNetworkInfoRTCommand { get; private set; }

        /// <summary>
        /// Networks the information clicked.
        /// </summary>
        private void NetworkInfoRTClicked()
        {
            App.RootFrame.Navigate(typeof(NetworkDetailPage), this.Network);
        }
    }
}
