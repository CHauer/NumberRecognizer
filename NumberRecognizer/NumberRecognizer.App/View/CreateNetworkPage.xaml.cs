//-----------------------------------------------------------------------
// <copyright file="CreateNetworkPage.xaml.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Create Network Page.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.View
{
    using System;
    using System.Collections.ObjectModel;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.ViewModel;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CreateNetworkPage : Page
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
        /// The text block collection.
        /// </summary>
        private ObservableCollection<TextBlock> textBlockCollection = new ObservableCollection<TextBlock>();

        /// <summary>
        /// The view model.
        /// </summary>
        private CreateNetworkPageViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNetworkPage"/> class.
        /// </summary>
        public CreateNetworkPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.viewModel = (CreateNetworkPageViewModel)this.DataContext;
            this.InitializePage();
            this.inkCanvasRTStackPanel.SizeChanged += this.InkCanvasRTStackPanel_SizeChanged;
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
            get { return this.navigationHelper; }
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
        /// Initializes the page.
        /// </summary>
        private void InitializePage()
        {
            foreach (InkCanvasRT inkCanvasRT in this.viewModel.InkCanvasRTCollection)
            {
                this.inkCanvasRTStackPanel.Children.Add(inkCanvasRT);
                TextBlock textBlock = new TextBlock()
                {
                    Text = string.Format("{0} *", inkCanvasRT.Name),
                    FontSize = 28,
                    Foreground = new SolidColorBrush(Colors.OrangeRed),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = inkCanvasRT.Margin
                };
                this.textBlockCollection.Add(textBlock);
                this.textBlockStackPanel.Children.Add(textBlock);
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the InkCanvasRTStackPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void InkCanvasRTStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            int count = this.viewModel.InkCanvasRTCollection.Count;

            foreach (InkCanvasRT inkCanvasRT in this.viewModel.InkCanvasRTCollection)
            {
                inkCanvasRT.Height = stackPanel.ActualHeight / count;
                inkCanvasRT.Width = stackPanel.ActualWidth;
            }

            foreach (TextBlock textBlock in this.textBlockCollection)
            {
                textBlock.Height = stackPanel.ActualHeight / count;
                textBlock.Width = stackPanel.ActualWidth;
            }

            this.inkCanvasRTStackPanel.SizeChanged -= this.InkCanvasRTStackPanel_SizeChanged;
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
