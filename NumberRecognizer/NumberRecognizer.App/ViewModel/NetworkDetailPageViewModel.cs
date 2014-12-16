//-----------------------------------------------------------------------
// <copyright file="NetworkDetailPageViewModel.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Detail Page ViewModel.</summary>
//-----------------------------------------------------------------------

using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace NumberRecognizer.App.ViewModel
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using System.Diagnostics;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;
    using System.Collections.ObjectModel;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.Control;
    using NumberRecognition.Labeling;
    using System.Threading.Tasks;

    /// <summary>
    /// Network Detail Page ViewModel.
    /// </summary>
    [ImplementPropertyChanged]
    public class NetworkDetailPageViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDetailPageViewModel"/> class.
        /// </summary>
        /// <param name="network">The network.</param>
        public NetworkDetailPageViewModel(NetworkInfo network)
        {
            this.Network = network;
            
            FinalPoolFitnessTrend = new ObservableCollection<ChartPopulation>();
            MultiplePoolFitnessTrends = new ObservableCollection<ObservableCollection<ChartPopulation>>();
            this.InitializeProperties();
            this.InitializeCommands();
        }

        /// <summary>
        /// Gets or sets the network.
        /// </summary>
        /// <value>
        /// The network.
        /// </value>
        public NetworkInfo Network { get; set; }

        /// <summary>
        /// Gets or sets the chart fittness trend.
        /// </summary>
        /// <value>
        /// The chart fittness trend.
        /// </value>
        public ObservableCollection<ChartPopulation> FinalPoolFitnessTrend { get; set; }

        /// <summary>
        /// Gets or sets the multiple pool fitness trends.
        /// </summary>
        /// <value>
        /// The multiple pool fitness trends.
        /// </value>
        public ObservableCollection<ObservableCollection<ChartPopulation>> MultiplePoolFitnessTrends { get; set; }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            
        }

        /// <summary>
        /// Loads the network details.
        /// </summary>
        private async Task<NetworkInfo> LoadNetworkDetails()
        {
            try
            {
                NumberRecognizerServiceClient proxyClient = new NumberRecognizerServiceClient();

                return await proxyClient.GetNetworkDetailAsync(Network.NetworkId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private async void InitializeProperties()
        {
            Network = await this.LoadNetworkDetails();

            if (Network.Calculated && Network.FinalPoolFitnessLog != null)
            {
                for (int generationNr = 0; generationNr < Network.FinalPoolFitnessLog.FitnessTrend.Count; generationNr++)
                {
                    FinalPoolFitnessTrend.Add(new ChartPopulation() { Name = generationNr.ToString(), Value = Network.FinalPoolFitnessLog.FitnessTrend[generationNr] * 100});
                }
            }

            if (Network.Calculated && Network.MultiplePoolFitnessLog != null && Network.MultiplePoolFitnessLog.Count > 0)
            {
                
                foreach (var pool in Network.MultiplePoolFitnessLog.Values)
                {
                    ObservableCollection<ChartPopulation> list = new ObservableCollection<ChartPopulation>();

                    //todo change to count (bugfix service)
                    for (int generationNr = 0; generationNr < 50; generationNr++)
                    {
                        list.Add(new ChartPopulation() { Name = generationNr.ToString(), Value = pool.FitnessTrend[generationNr] * 100 });
                    }

                    MultiplePoolFitnessTrends.Add(list);
                }
            }
        }
    }
}
