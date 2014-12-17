//-----------------------------------------------------------------------
// <copyright file="DesignGroupedNetworksPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Hauer Christoph. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>Design Grouped Networks Page ViewModel.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.ViewModel.Design
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;

    /// <summary>
    /// Grouped Networks Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class DesignGroupedNetworksPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The selected network.
        /// </summary>
        private NetworkInfo selectedLocalNetwork;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignGroupedNetworksPageViewModel"/> class.
        /// </summary>
        public DesignGroupedNetworksPageViewModel()
        {
            this.Initialize();
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
        /// Gets the toggle synchronize command.
        /// </summary>
        /// <value>
        /// The toggle synchronize command.
        /// </value>
        public ICommand RefreshCommand { get; private set; }

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
        private async void Initialize()
        {
            this.LoadDesignDataNetworksAsync();
        }

        /// <summary>
        /// Loads the design data networks asynchronous.
        /// </summary>
        private void LoadDesignDataNetworksAsync()
        {
            this.NetworkGroups = new ObservableCollection<NetworkInfoGroup>();
            NetworkInfoGroup calculated = new NetworkInfoGroup("calculated", "Calculated Networks");
            calculated.Networks.Add(new NetworkInfo()
            {
                Calculated = true,
                CalculationEnd = DateTime.Now,
                CalculationStart = DateTime.Now.Subtract(new TimeSpan(0, 10, 0)),
                NetworkId = 1,
                NetworkFitness = 0.99,
                NetworkName = "TestCalculated",
                Status = NetworkStatusType.Ready,
                FinalPatternFittness = new Dictionary<string, double>() { { "0", 0.99 }, { "1", 0.99 }, { "2", 0.99 }, { "3", 0.99 }, { "4", 0.99 }, { "5", 0.99 }, { "6", 0.99 }, { "7", 0.99 }, { "8", 0.99 }, { "9", 0.99 } },
                FinalPoolFitnessLog = new FitnessLog(),
                ChartFitness = new ObservableCollection<ChartPopulation>() { new ChartPopulation { Name = "0", Value = 0.96 * 100 }, new ChartPopulation { Name = "1", Value = 0.92 * 100 }, new ChartPopulation { Name = "2", Value = 0.96 * 100 }, new ChartPopulation { Name = "3", Value = 0.34 * 100 }, new ChartPopulation { Name = "4", Value = 0.96 * 100 }, new ChartPopulation { Name = "5", Value = 0.87 * 100 }, new ChartPopulation { Name = "6", Value = 0.76 * 100 }, new ChartPopulation { Name = "7", Value = 0.73 * 100 }, new ChartPopulation { Name = "8", Value = 0.95 * 100 }, new ChartPopulation { Name = "9", Value = 0.98 * 100 } }
            });

            calculated.Networks.Add(new NetworkInfo()
            {
                Calculated = true,
                CalculationEnd = DateTime.Now,
                CalculationStart = DateTime.Now.Subtract(new TimeSpan(0, 10, 0)),
                NetworkId = 1,
                NetworkFitness = 0.99,
                NetworkName = "TestCalculated",
                Status = NetworkStatusType.Ready,
                FinalPatternFittness = new Dictionary<string, double>() { { "0", 0.99 }, { "1", 0.99 }, { "2", 0.99 }, { "3", 0.99 }, { "4", 0.99 }, { "5", 0.99 }, { "6", 0.99 }, { "7", 0.99 }, { "8", 0.99 }, { "9", 0.99 } },
                FinalPoolFitnessLog = new FitnessLog(),
                ChartFitness = new ObservableCollection<ChartPopulation>() { new ChartPopulation { Name = "0", Value = 0.96 * 100 }, new ChartPopulation { Name = "1", Value = 0.92 * 100 }, new ChartPopulation { Name = "2", Value = 0.96 * 100 }, new ChartPopulation { Name = "3", Value = 0.34 * 100 }, new ChartPopulation { Name = "4", Value = 0.96 * 100 }, new ChartPopulation { Name = "5", Value = 0.87 * 100 }, new ChartPopulation { Name = "6", Value = 0.76 * 100 }, new ChartPopulation { Name = "7", Value = 0.73 * 100 }, new ChartPopulation { Name = "8", Value = 0.95 * 100 }, new ChartPopulation { Name = "9", Value = 0.98 * 100 } }
            });

            NetworkInfoGroup uncalculated = new NetworkInfoGroup("uncalculated", "Uncalculated Networks");
            uncalculated.Networks.Add(new NetworkInfo()
            {
                Calculated = false,
                CalculationEnd = null,
                CalculationStart = null,
                NetworkId = 1,
                NetworkName = "TestUncalculated",
                Status = NetworkStatusType.NotStarted
            });

            this.NetworkGroups.Add(calculated);
            this.NetworkGroups.Add(uncalculated);
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.RefreshCommand = new RelayCommand(() => this.LoadDesignDataNetworksAsync());
        }
    }
}
