//-----------------------------------------------------------------------
// <copyright file="GroupDetailPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Group Detail Page ViewModel.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.ViewModel
{
    using System;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.NumberRecognizerService;
    using PropertyChanged;
    using Windows.UI.Popups;
    using System.Windows.Input;
    using Mutzl.MvvmLight;

    /// <summary>
    /// Group Detail Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class GroupDetailPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The selected network.
        /// </summary>
        private NetworkInfoRT selectedNetwork = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDetailPageViewModel"/> class.
        /// </summary>
        public GroupDetailPageViewModel()
        {
            this.InitializeProperties();
            this.InitializeCommands();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDetailPageViewModel"/> class.
        /// </summary>
        /// <param name="networkGroup">The network group.</param>
        public GroupDetailPageViewModel(NetworkInfoGroup networkGroup)
            : this()
        {
            this.NetworkGroup = networkGroup;
        }

        /// <summary>
        /// Gets or sets the network group.
        /// </summary>
        /// <value>
        /// The network group.
        /// </value>
        public NetworkInfoGroup NetworkGroup { get; set; }

        /// <summary>
        /// Gets or sets the delete network command.
        /// </summary>
        /// <value>
        /// The delete network command.
        /// </value>
        /// 
        public ICommand DeleteNetworkCommand { get; set; }

        /// <summary>
        /// Gets or sets the selected network.
        /// </summary>
        /// <value>
        /// The selected network.
        /// </value>
        public NetworkInfoRT SelectedNetwork
        {
            get
            {
                return this.selectedNetwork;
            }

            set
            {
                this.SelectedNetwork = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is network selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is network selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsNetworkSelected
        {
            get
            {
                return this.selectedNetwork != null;
            }
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.DeleteNetworkCommand = new DependentRelayCommand(this.DeleteNetwork, () => this.SelectedNetwork != null, this, () => this);
        }

        /// <summary>
        /// Deletes the network.
        /// </summary>
        private async void DeleteNetwork()
        {
            MessageDialog msgDialog = new MessageDialog("Do you really want to delete this Network?", this.SelectedNetwork.Network.NetworkName);

            UICommand ok = new UICommand("OK");
            ok.Invoked = async delegate(IUICommand command)
            {
                NumberRecognizerServiceClient serviceClient = new NumberRecognizerServiceClient();
                await serviceClient.DeleteNetworkAsync(this.SelectedNetwork.Network.NetworkId);
                this.NetworkGroup.Networks.Remove(this.SelectedNetwork);
                this.SelectedNetwork = null;
            };
            msgDialog.Commands.Add(ok);

            //Cancel Button
            UICommand cancel = new UICommand("Cancel");
            msgDialog.Commands.Add(cancel);

            await msgDialog.ShowAsync();
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void InitializeProperties()
        {
        }
    }
}
