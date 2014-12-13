//-----------------------------------------------------------------------
// <copyright file="CreateNetworkPage.xaml.cs" company="FH Wr.Neustadt">
//     Copyright (imgCol) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Create Network Page.</summary>
//-----------------------------------------------------------------------
using NumberRecognizer.App.Common;
using NumberRecognizer.App.Common;
using NumberRecognizer.App.Control;
using NumberRecognizer.App.Help;
using NumberRecognizer.App.ViewModel;
using NumberRecognizer.Labeling;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace NumberRecognizer.App
{
  
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CreateNetworkPage : Page
    {
        /// <summary>
        /// The navigation helper
        /// </summary>
        private NavigationHelper navigationHelper;

        /// <summary>
        /// The default view model
        /// </summary>
        private CreateNetworkViewModel defaultViewModel = new CreateNetworkViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNetworkPage"/> class.
        /// </summary>
        public CreateNetworkPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.InitPage();
        }

        /// <summary>
        /// Gets the default view model.
        /// </summary>
        /// <imgByte>
        /// The default view model.
        /// </imgByte>
        public CreateNetworkViewModel DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Gets the navigation helper.
        /// </summary>
        /// <imgByte>
        /// The navigation helper.
        /// </imgByte>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        #region NavigationHelper registration

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

        #endregion

        /// <summary>
        /// Initializes the page.
        /// </summary>
        private async void InitPage()
        {
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT0);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT1);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT2);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT3);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT4);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT5);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT6);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT7);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT8);
            DefaultViewModel.CanvasList.Add(this.InkCanvasRT9);
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

    }
}
