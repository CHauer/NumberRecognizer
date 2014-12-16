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
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;
    using System.Collections.ObjectModel;
    using NumberRecognizer.App.Help;
    using NumberRecognizer.App.Control;
    using NumberRecognition.Labeling;

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
        /// Initializes the properties.
        /// </summary>
        private void InitializeProperties()
        {
            FinalPoolFitnessTrend = new ObservableCollection<ChartPopulation>();

            if (Network.Calculated && Network.FinalPoolFitnessLog != null)
            {
                for (int generationNr = 0; generationNr < Network.FinalPoolFitnessLog.FitnessTrend.Count; generationNr++)
                {
                    FinalPoolFitnessTrend.Add(new ChartPopulation() { Name = generationNr.ToString(), Value = Network.FinalPoolFitnessLog.FitnessTrend[generationNr] });
                }
            }

            if (Network.Calculated && Network.MultiplePoolFitnessLog != null && Network.MultiplePoolFitnessLog.Count > 0)
            {
                foreach (var pool in Network.MultiplePoolFitnessLog.Values)
                {
                    for (int generationNr = 0; generationNr < pool.FitnessTrend.Count; generationNr++)
                    {
                        FinalPoolFitnessTrend.Add(new ChartPopulation() { Name = generationNr.ToString(), Value = pool.FitnessTrend[generationNr] });
                    }
                }
            }
        }
    }
}
