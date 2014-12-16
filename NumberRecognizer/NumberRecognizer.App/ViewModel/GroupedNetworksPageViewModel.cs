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

    /// <summary>
    /// Grouped Networks Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class GroupedNetworksPageViewModel : ViewModelBase
    {

        /// <summary>
        /// The selected network.
        /// </summary>
        private NetworkInfo selectedLocalNetwork;

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
                return this.selectedLocalNetwork != null;
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
        /// Gets the network clicked command.
        /// </summary>
        /// <value>
        /// The network clicked.
        /// </value>
        public RelayCommand<NetworkInfo> NetworkClicked { get; private set; }

        /// <summary>
        /// Gets or sets the selected network.
        /// </summary>
        /// <value>
        /// The selected network.
        /// </value>
        public NetworkInfo SelectedLocalNetwork
        {
            get
            {
                return this.selectedLocalNetwork;
            }

            set
            {
                this.selectedLocalNetwork = value;
                this.RaisePropertyChanged(() => this.IsNetworkSelected);
            }
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private async void InitializeProperties()
        {
            await this.LoadNetworksAsync();
        }

        /// <summary>
        /// Synchronizes the networks asynchronous.
        /// </summary>
        /// <returns>The networks asynchronous.</returns>
        private async Task LoadNetworksAsync()
        {
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
            foreach (NetworkInfo network in this.Networks.Where(p => p.Calculated))
            {
                network.ChartFitness = new List<ChartPopulation>();
                foreach (string key in network.FinalPoolFitnessLog.FinalPatternFittness.Keys)
                {
                    network.ChartFitness.Add(new ChartPopulation()
                    {
                        Name = key,
                        Value = network.FinalPoolFitnessLog.FinalPatternFittness[key] * 100
                    });
                }
                calculated.Networks.Add(network);
            }

            NetworkInfoGroup uncalculated = new NetworkInfoGroup("uncalculated", "Uncalculated Networks");
            foreach (NetworkInfo network in this.Networks.Where(p => !p.Calculated))
            {
                uncalculated.Networks.Add(network);
            }

            this.NetworkGroups.Add(calculated);
            this.NetworkGroups.Add(uncalculated);
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.CreateNetworkCommand = new DependentRelayCommand(() => App.RootFrame.Navigate(typeof(CreateNetworkPage)),
                                                                  () => this.SelectedLocalNetwork == null, this,
                                                                  () => this.SelectedLocalNetwork);
            this.DeleteNetworkCommand = new DependentRelayCommand(this.DeleteNetwork,
                                                                  () => this.SelectedLocalNetwork != null, this,
                                                                  () => this.SelectedLocalNetwork);
            this.RefreshCommand = new RelayCommand(() => LoadNetworksAsync());
            this.NetworkClicked = new RelayCommand<NetworkInfo>((item) => App.RootFrame.Navigate(typeof(NetworkDetailPage), item));
        }

        /// <summary>
        /// Deletes the network.
        /// </summary>
        private async void DeleteNetwork()
        {
            MessageDialog msgDialog = new MessageDialog("Do you really want to delete this Network?", this.SelectedLocalNetwork.NetworkName);

            UICommand ok = new UICommand("OK");
            ok.Invoked = async delegate(IUICommand command)
            {
                NumberRecognizerServiceClient serviceClient = new NumberRecognizerServiceClient();
                await serviceClient.DeleteNetworkAsync(this.SelectedLocalNetwork.NetworkId);
                this.SelectedLocalNetwork = null;
            };
            msgDialog.Commands.Add(ok);

            UICommand cancel = new UICommand("Cancel");
            msgDialog.Commands.Add(cancel);

            await msgDialog.ShowAsync();
            await this.LoadNetworksAsync();
        }
    }
}
