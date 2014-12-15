//-----------------------------------------------------------------------
// <copyright file="GroupedNetworksPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Data Source.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.App.View;
    using PropertyChanged;
    using Windows.UI.Xaml;
    using Mutzl.MvvmLight;
    using Windows.UI.Popups;
    using Windows.Foundation;

    /// <summary>
    /// Grouped Networks Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class GroupedNetworksPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The is synchronize enabled.
        /// </summary>
        private static bool isSyncEnabled = false;

        /// <summary>
        /// The selected network.
        /// </summary>
        private NetworkInfoRT selectedNetwork;

        /// <summary>
        /// The dispatcher timer.
        /// </summary>
        private DispatcherTimer dispatcherTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedNetworksPageViewModel"/> class.
        /// </summary>
        public GroupedNetworksPageViewModel()
        {
            this.InitializeProperties();
            this.InitializeCommands();
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
        /// Gets or sets the networks.
        /// </summary>
        /// <value>
        /// The networks.
        /// </value>
        public ObservableCollection<NetworkInfo> Networks { get; set; }

        /// <summary>
        /// Gets or sets the network groups.
        /// </summary>
        /// <value>
        /// The network groups.
        /// </value>
        public ObservableCollection<NetworkInfoGroup> NetworkGroups { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is synchronize enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is synchronize enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsSyncEnabled
        {
            get
            {
                return isSyncEnabled;
            }

            set
            {
                if (isSyncEnabled = value)
                {
                    this.dispatcherTimer.Start();
                }
                else if (this.dispatcherTimer.IsEnabled)
                {
                    this.dispatcherTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Gets the create network command.
        /// </summary>
        /// <value>
        /// The create network command.
        /// </value>
        public ICommand CreateNetworkCommand { get; private set; }

        /// <summary>
        /// Gets the delete network command.
        /// </summary>
        /// <value>
        /// The delete network command.
        /// </value>
        public ICommand DeleteNetworkCommand { get; private set; }

        /// <summary>
        /// Gets the toggle synchronize command.
        /// </summary>
        /// <value>
        /// The toggle synchronize command.
        /// </value>
        public ICommand ToggleSyncCommand { get; private set; }

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
                this.selectedNetwork = value;
                this.RaisePropertyChanged(() => this.IsNetworkSelected);
            }
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private async void InitializeProperties()
        {
            await this.SyncNetworksAsync();
            this.InitializeDispatcherTimer();
        }

        /// <summary>
        /// Initializes the dispatcher timer.
        /// </summary>
        private void InitializeDispatcherTimer()
        {
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += this.DispatcherTimer_Tick;
            this.dispatcherTimer.Interval = new System.TimeSpan(0, 0, 5);
            if (isSyncEnabled)
            {
                this.dispatcherTimer.Start();
            }
        }

        /// <summary>
        /// Dispatchers the timer_ tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private async void DispatcherTimer_Tick(object sender, object e)
        {
            if (isSyncEnabled)
            {
                await this.SyncNetworksAsync();
            }
            else if (this.dispatcherTimer.IsEnabled)
            {
                this.dispatcherTimer.Stop();
            }
        }

        /// <summary>
        /// Synchronizes the networks asynchronous.
        /// </summary>
        /// <returns>The networks asynchronous.</returns>
        private async Task SyncNetworksAsync()
        {
            NumberRecognizerServiceClient serviceClient = new NumberRecognizerServiceClient();
            this.Networks = await serviceClient.GetNetworksAsync();

            this.NetworkGroups = new ObservableCollection<NetworkInfoGroup>();
            NetworkInfoGroup calculated = new NetworkInfoGroup("calculated", "Calculated Networks");
            foreach (NetworkInfo network in this.Networks.Where(p => p.Calculated))
            {
                calculated.Networks.Add(new NetworkInfoRT(network));
            }

            NetworkInfoGroup uncalculated = new NetworkInfoGroup("uncalculated", "Uncalculated Networks");
            foreach (NetworkInfo network in this.Networks.Where(p => !p.Calculated))
            {
                uncalculated.Networks.Add(new NetworkInfoRT(network));
            }

            this.NetworkGroups.Add(calculated);
            this.NetworkGroups.Add(uncalculated);
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.CreateNetworkCommand = new DependentRelayCommand(() => App.RootFrame.Navigate(typeof(CreateNetworkPage)), () => this.SelectedNetwork == null, this, () => this.SelectedNetwork);
            this.DeleteNetworkCommand = new DependentRelayCommand(this.DeleteNetwork, () => this.SelectedNetwork != null, this, () => SelectedNetwork);
            this.ToggleSyncCommand = new RelayCommand(() => this.IsSyncEnabled = !this.IsSyncEnabled);
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
                this.SelectedNetwork = null;
            };
            msgDialog.Commands.Add(ok);

            //Cancel Button
            UICommand cancel = new UICommand("Cancel");
            msgDialog.Commands.Add(cancel);

            await msgDialog.ShowAsync();
            await this.SyncNetworksAsync();
        }
    }
}
