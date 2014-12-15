//-----------------------------------------------------------------------
// <copyright file="GroupedImagesPage.xaml.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Grouped Images Page.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.View
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.ViewModel;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedImagesPage : Page
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
        private GroupedImagesPageViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedImagesPage"/> class.
        /// </summary>
        public GroupedImagesPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        /// <summary>
        /// Gets the default view model.
        /// </summary>
        /// <value>
        /// The default view model.
        /// </value>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Gets the navigation helper.
        /// </summary>
        /// <value>
        /// The navigation helper.
        /// </value>
        public NavigationHelper NavigationHelper
        {
            get
            {
                return this.navigationHelper;
            }
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will load the current Page. Usually the most relevant property to examine is Parameter.</param>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="GridCS.Common.NavigationHelper.LoadState" />
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState" />.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
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
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            this.viewModel = (GroupedImagesPageViewModel)this.DataContext;
            KeyValuePair<string, ObservableCollection<TrainingImageRT>> keyValuePair = (KeyValuePair<string, ObservableCollection<TrainingImageRT>>)e.NavigationParameter;
            this.viewModel.NetworkName = keyValuePair.Key;
            this.viewModel.Images = keyValuePair.Value;
        }
    }
}
