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
    using Windows.UI.Xaml.Controls;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using Mutzl.MvvmLight;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.Cloud.Contract.Data;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.App.View;
    using PropertyChanged;
    using Windows.UI.Popups;
    using System.Collections.Generic;
    using GalaSoft.MvvmLight.Command;
    using Windows.UI.Xaml;
    using Windows.UI.Core;

    /// <summary>
    /// Grouped Networks Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class GroupedNetworksPageViewModel : ViewModelBase
    {

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
        /// Gets a value indicating whether this instance is network selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is network selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsAppBarOpen { get; set; }

        /// <summary>
        /// Gets a value indicating whether is loading networks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is loading]; otherwise, <c>false</c>.
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
        /// Initializes the properties.
        /// </summary>
        private async void InitializeProperties()
        {
            await this.LoadNetworksAsync();
            this.SelectedNetwork = null;
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
            foreach (NetworkInfo network in this.Networks.Where(p => p.Status != NetworkStatusType.Ready && p.Status != NetworkStatusType.Running))
            {
                uncalculated.Networks.Add(network);
            }

            this.NetworkGroups.Add(calculated);
            this.NetworkGroups.Add(runnning);
            this.NetworkGroups.Add(uncalculated);

            this.IsLoading = false;
            RaisePropertyChanged(() => IsLoading);
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.CreateNetworkCommand = new DependentRelayCommand(() => App.RootFrame.Navigate(typeof(CreateNetworkPage)),
                                                                  () => this.SelectedNetwork == null, this,
                                                                  () => this.SelectedNetwork);
            this.DeleteNetworkCommand = new DependentRelayCommand(this.DeleteNetwork,
                                                                  () => this.SelectedNetwork != null, this,
                                                                  () => this.SelectedNetwork);
            this.RefreshCommand = new RelayCommand(() => LoadNetworksAsync());
            this.NetworkClicked = new RelayCommand<NetworkInfo>((item) => App.RootFrame.Navigate(typeof(NetworkRecognizePage), item));
            this.NetworkDetails = new DependentRelayCommand(() =>App.RootFrame.Navigate(typeof(NetworkDetailPage), SelectedNetwork),
                                                            () => this.SelectedNetwork != null && this.SelectedNetwork.Calculated, this,
                                                            () => this.SelectedNetwork);

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
                NumberRecognizerServiceClient serviceClient = new NumberRecognizerServiceClient();
                await serviceClient.DeleteNetworkAsync(this.SelectedNetwork.NetworkId);
                this.SelectedNetwork = null;

                //refresh data
                await this.LoadNetworksAsync();
            };

            msgDialog.Commands.Add(ok);

            UICommand cancel = new UICommand("Cancel");
            msgDialog.Commands.Add(cancel);

            await msgDialog.ShowAsync();
        }
    }
}
