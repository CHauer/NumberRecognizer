using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NumberRecognizer.App.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage für die Seite "Gruppierte Elemente" ist unter http://go.microsoft.com/fwlink/?LinkId=234231 dokumentiert.
using NumberRecognizer.App.DataModel;
using NumberRecognizer.App.ViewModel;

namespace NumberRecognizer.App.View
{
    /// <summary>
    /// Eine Seite, auf der eine gruppierte Auflistung von Elementen angezeigt wird.
    /// </summary>
    public sealed partial class ValidateTrainingDataPage : Page
    {
        private NavigationHelper navigationHelper;

        /// <summary>
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ValidateTrainingDataViewModel DefaultViewModel { get; set; }

        /// <summary>
        /// NavigationHelper wird auf jeder Seite zur Unterstützung bei der Navigation verwendet und 
        /// Verwaltung der Prozesslebensdauer
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get
            {
                return this.navigationHelper;
            }
        }

        public ValidateTrainingDataPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird.  Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var localTrainingData = (Dictionary<int, List<LocalTrainingImage>>)e.NavigationParameter;

            DefaultViewModel = new ValidateTrainingDataViewModel(localTrainingData);

            this.DataContext = DefaultViewModel;
        }

        #region NavigationHelper-Registrierung

        /// Die in diesem Abschnitt bereitgestellten Methoden lassen sich einfach für Folgendes verwenden:
        /// Reaktion von NavigationHelper auf die Navigationsmethoden der Seite.
        /// 
        /// Platzieren von seitenspezifischer Logik in Ereignishandlern für  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// und <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// Der Navigationsparameter ist in der LoadState-Methode verfügbar 
        /// zusätzlich zum Seitenzustand, der während einer früheren Sitzung beibehalten wurde.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
