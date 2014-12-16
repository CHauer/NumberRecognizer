//-----------------------------------------------------------------------
// <copyright file="NetworkDetailPage.xaml.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Item Detail Page.</summary>
//-----------------------------------------------------------------------

using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.App.View
{
    using System;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.App.ViewModel;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class NetworkDetailPage : Page
    {
        /// <summary>
        /// The navigation helper.
        /// </summary>
        private NavigationHelper navigationHelper;

        /// <summary>
        /// The default view model.
        /// </summary>
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// The view model.
        /// </summary>
        private NetworkDetailPageViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDetailPage"/> class.
        /// </summary>
        public NetworkDetailPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        /// <summary>
        /// Gets NavigationHelper is used on each page to aid in navigation and
        /// process lifetime management.
        /// </summary>
        /// <value>
        /// The navigation helper.
        /// </value>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets This can be changed to a strongly typed view model.
        /// </summary>
        /// <value>
        /// The default view model.
        /// </value>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will load the current Page. Usually the most relevant property to examine is Parameter.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        /// <summary>
        /// Invoked immediately after the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the navigation that has unloaded the current Page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            this.viewModel = (NetworkDetailPageViewModel)this.DataContext;
            this.viewModel.Network = (NetworkInfo)e.NavigationParameter;
            this.viewModel.InkCanvas = this.RecognitionInkCanvas;
        }
    }
}