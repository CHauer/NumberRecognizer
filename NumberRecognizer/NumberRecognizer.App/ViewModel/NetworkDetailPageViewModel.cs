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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;

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

            this.FinalPoolFitnessTrend = new ObservableCollection<ChartPopulation>();
            this.MultiplePoolFitnessTrends = new ObservableCollection<ObservableCollection<ChartPopulation>>();
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
        /// Gets or sets the chart fitness trend.
        /// </summary>
        /// <value>
        /// The chart fitness trend.
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
        /// <returns>The network details.</returns>
        private async Task<NetworkInfo> LoadNetworkDetails()
        {
            try
            {
                NumberRecognizerServiceClient proxyClient = new NumberRecognizerServiceClient();
                return await proxyClient.GetNetworkDetailAsync(this.Network.NetworkId);
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
            this.Network = await this.LoadNetworkDetails();

            if (this.Network.Calculated && this.Network.FinalPoolFitnessLog != null)
            {
                for (int generationNr = 0; generationNr < this.Network.FinalPoolFitnessLog.FitnessTrend.Count; generationNr++)
                {
                    this.FinalPoolFitnessTrend.Add(new ChartPopulation() { Name = generationNr.ToString(), Value = this.Network.FinalPoolFitnessLog.FitnessTrend[generationNr] * 100 });
                }
            }

            if (this.Network.Calculated && this.Network.MultiplePoolFitnessLog != null && this.Network.MultiplePoolFitnessLog.Count > 0)
            {
                foreach (var pool in this.Network.MultiplePoolFitnessLog.Values)
                {
                    ObservableCollection<ChartPopulation> list = new ObservableCollection<ChartPopulation>();

                    for (int generationNr = 0; generationNr < pool.FitnessTrend.Count; generationNr++)
                    {
                        list.Add(new ChartPopulation() { Name = generationNr.ToString(), Value = pool.FitnessTrend[generationNr] * 100 });
                    }

                    this.MultiplePoolFitnessTrends.Add(list);
                }
            }
        }
    }
}
