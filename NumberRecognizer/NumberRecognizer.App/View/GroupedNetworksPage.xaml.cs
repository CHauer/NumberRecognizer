//-----------------------------------------------------------------------
// <copyright file="GroupedNetworksPage.xaml.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Grouped Items Page.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.View
{
    using System;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.DataModel;
    using NumberRecognizer.App.ViewModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedNetworksPage : Page
    {
        /// <summary>
        /// The navigation helper.
        /// </summary>
        private NavigationHelper navigationHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedNetworksPage"/> class.
        /// </summary>
        public GroupedNetworksPage()
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
        /// Handles the LoadState event of the NavigationHelper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LoadStateEventArgs"/> instance containing the event data.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }
    }
}