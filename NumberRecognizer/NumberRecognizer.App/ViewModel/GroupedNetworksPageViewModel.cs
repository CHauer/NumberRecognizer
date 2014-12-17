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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Mutzl.MvvmLight;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.App.View;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

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
                return this.SelectedNetwork != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is application bar open.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is application bar open; otherwise, <c>false</c>.
        /// </value>
        public bool IsAppBarOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }

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
        /// Gets the create network command.
        /// </summary>
        /// <value>
        /// The create network command.
        /// </value>
        public ICommand CreateNetworkCommand { get; private set; }

        /// <summary>
        /// Gets the toggle synchronize command.
        /// </summary>
        /// <value>
        /// The toggle synchronize command.
        /// </value>
        public ICommand ToggleSyncCommand { get; private set; }

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
        public ICommand RefreshCommand { get; private set; }

        /// <summary>
        /// Gets the toggle synchronize command.
        /// </summary>
        /// <value>
        /// The toggle synchronize command.
        /// </value>
        public RelayCommand<SelectionChangedEventArgs> SelectionChanged { get; private set; }

        /// <summary>
        /// Gets the network clicked command.
        /// </summary>
        /// <value>
        /// The network clicked.
        /// </value>
        public RelayCommand<NetworkInfo> NetworkClicked { get; private set; }

        /// <summary>
        /// Gets the network clicked command.
        /// </summary>
        /// <value>
        /// The network clicked.
        /// </value>
        public DependentRelayCommand NetworkDetails { get; private set; }

        /// <summary>
        /// Gets or sets the selected network.
        /// </summary>
        /// <value>
        /// The selected network.
        /// </value>
        [AlsoNotifyFor("IsNetworkSelected")]
        public NetworkInfo SelectedNetwork { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is synchronize enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is synchronize enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSyncEnabled
        {
            get
            {
                return isSyncEnabled;
            }

            set
            {
                isSyncEnabled = value;
                if (isSyncEnabled)
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
        /// Initializes the properties.
        /// </summary>
        private async void InitializeProperties()
        {
            await this.LoadNetworksAsync();
            this.InitializeDispatcherTimer();
            this.SelectedNetwork = null;
        }

        /// <summary>
        /// Initializes the dispatcher timer.
        /// </summary>
        private void InitializeDispatcherTimer()
        {
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += this.DispatcherTimer_Tick;
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 20);
            if (isSyncEnabled)
            {
                this.dispatcherTimer.Start();
            }
        }

        /// <summary>
        /// Dispatchers the timer_ tick.
        /// </summary>
        /// <param name="sender">The sender parameter.</param>
        /// <param name="e">The event parameter.</param>
        private async void DispatcherTimer_Tick(object sender, object e)
        {
            if (isSyncEnabled && !this.IsLoading)
            {
                await this.LoadNetworksAsync();
            }
            else if (this.dispatcherTimer.IsEnabled && !isSyncEnabled)
            {
                this.dispatcherTimer.Stop();
            }
        }

        /// <summary>
        /// Synchronizes the networks asynchronous.
        /// </summary>
        /// <returns>The networks asynchronous.</returns>
        private async Task LoadNetworksAsync()
        {
            this.IsLoading = true;

            try
            {
                NumberRecognizerServiceClient serviceClient = new NumberRecognizerServiceClient();
                this.Networks = await serviceClient.GetNetworksAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }

            this.CreateNetworkGroups();

            this.IsLoading = false;
            this.RaisePropertyChanged(() => this.IsLoading);
        }

        /// <summary>
        /// Creates the network groups.
        /// </summary>
        private void CreateNetworkGroups()
        {
            this.NetworkGroups = new ObservableCollection<NetworkInfoGroup>();
            NetworkInfoGroup calculated = new NetworkInfoGroup("calculated", "Calculated Networks");
            foreach (NetworkInfo network in this.Networks.Where(p => p.Status == NetworkStatusType.Ready))
            {
                network.ChartFitness = new List<ChartPopulation>();
                foreach (string key in network.FinalPatternFittness.Keys)
                {
                    network.ChartFitness.Add(new ChartPopulation()
                    {
                        Name = key,
                        Value = network.FinalPatternFittness[key] * 100
                    });
                }

                calculated.Networks.Add(network);
            }

            NetworkInfoGroup runnning = new NetworkInfoGroup("running", "Calculation Running");
            foreach (NetworkInfo network in this.Networks.Where(p => p.Status == NetworkStatusType.Running))
            {
                runnning.Networks.Add(network);
            }

            NetworkInfoGroup uncalculated = new NetworkInfoGroup("uncalculated", "Uncalculated Networks");
            foreach (
                NetworkInfo network in
                    this.Networks.Where(p => p.Status != NetworkStatusType.Ready && p.Status != NetworkStatusType.Running))
            {
                uncalculated.Networks.Add(network);
            }

            this.NetworkGroups.Add(calculated);
            this.NetworkGroups.Add(runnning);
            this.NetworkGroups.Add(uncalculated);
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.CreateNetworkCommand = new DependentRelayCommand(() => App.RootFrame.Navigate(typeof(CreateNetworkPage)), () => this.SelectedNetwork == null, this, () => this.SelectedNetwork);
            this.DeleteNetworkCommand = new DependentRelayCommand(this.DeleteNetwork, () => this.SelectedNetwork != null, this, () => this.SelectedNetwork);
            this.RefreshCommand = new DependentRelayCommand(() => this.LoadNetworksAsync(), () => this.IsLoading == false && this.IsSyncEnabled == false, this, () => this.IsLoading, () => this.IsSyncEnabled);
            this.NetworkClicked = new RelayCommand<NetworkInfo>((item) => App.RootFrame.Navigate(typeof(NetworkRecognizePage), item), (item) => item.Status == NetworkStatusType.Ready);
            this.NetworkDetails = new DependentRelayCommand(() => App.RootFrame.Navigate(typeof(NetworkDetailPage), this.SelectedNetwork), () => this.SelectedNetwork != null && this.SelectedNetwork.Calculated, this, () => this.SelectedNetwork);
            this.ToggleSyncCommand = new RelayCommand(() => this.IsSyncEnabled = !this.IsSyncEnabled);
            this.SelectionChanged = new RelayCommand<SelectionChangedEventArgs>((args) =>
            {
                if (args.AddedItems.Count > 0)
                {
                    IsAppBarOpen = true;
                }

                if (args.RemovedItems.Count > 0)
                {
                    IsAppBarOpen = false;
                }
            });
        }

        /// <summary>
        /// Deletes the network.
        /// </summary>
        private async void DeleteNetwork()
        {
            MessageDialog msgDialog = new MessageDialog("Do you really want to delete this Network?", this.SelectedNetwork.NetworkName);

            UICommand ok = new UICommand("OK");
            ok.Invoked = async delegate(IUICommand command)
            {
                this.IsLoading = true;
                int delnetworkid = this.SelectedNetwork.NetworkId;
                this.SelectedNetwork.Calculated = false;
                this.SelectedNetwork.Status = NetworkStatusType.NotStarted;
                this.CreateNetworkGroups();
                NumberRecognizerServiceClient serviceClient = new NumberRecognizerServiceClient();
                await serviceClient.DeleteNetworkAsync(delnetworkid);
                this.SelectedNetwork = null;
                this.IsLoading = false;

                ////refresh data
                await this.LoadNetworksAsync();
            };

            msgDialog.Commands.Add(ok);

            UICommand cancel = new UICommand("Cancel");
            msgDialog.Commands.Add(cancel);

            await msgDialog.ShowAsync();
        }
    }
}
