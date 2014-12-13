//-----------------------------------------------------------------------
// <copyright file="CreateNetworkPage.xaml.cs" company="FH Wr.Neustadt">
//     Copyright (imgCol) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Create Network Page.</summary>
//-----------------------------------------------------------------------



namespace NumberRecognizer.App
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.WindowsRuntime;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.Help;
    using Windows.System.UserProfile;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;
    using Windows.UI.Xaml.Shapes;
    using NumberRecognizer.Labeling;

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
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// The ink canvases
        /// </summary>
        private List<InkCanvasRT> inkCanvasRT = new List<InkCanvasRT>();

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
        public ObservableDictionary DefaultViewModel
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
            this.inkCanvasRT.Add(this.InkCanvasRT0);
            this.inkCanvasRT.Add(this.InkCanvasRT1);
            this.inkCanvasRT.Add(this.InkCanvasRT2);
            this.inkCanvasRT.Add(this.InkCanvasRT3);
            this.inkCanvasRT.Add(this.InkCanvasRT4);
            this.inkCanvasRT.Add(this.InkCanvasRT5);
            this.inkCanvasRT.Add(this.InkCanvasRT6);
            this.inkCanvasRT.Add(this.InkCanvasRT7);
            this.inkCanvasRT.Add(this.InkCanvasRT8);
            this.inkCanvasRT.Add(this.InkCanvasRT9);

            this.NameTextBox.Text = string.Format("{0}_{1}", await UserInformation.GetLastNameAsync(), DateTime.Now.ToFileTime());
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

        /// <summary>
        /// Handles the Click event of the NextButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private async void NextButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (InkCanvasRT inkCanvas in this.inkCanvasRT)
            {
                var renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(inkCanvas);

                byte[] canvasRGBABytes = (await renderTargetBitmap.GetPixelsAsync()).ToArray();
                byte[] canvasBytes = await ImageHelperRT.GetByteArrayFromRGBAByteArrayAsync(canvasRGBABytes, inkCanvas.ForegroundColor);

                double canvasWidth = inkCanvas.ActualWidth;
                double canvasHeight = inkCanvas.ActualHeight;
                double[,] canvasPixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(canvasBytes, canvasWidth, canvasHeight);

                inkCanvas.ConnectedComponentLabeling = new ConnectedComponentLabeling(canvasPixels, 0.0);

                foreach (ConnectedComponent connectedComponent in inkCanvas.ConnectedComponentLabeling.ConnectedComponents)
                {
                    MinimumBoundingRectangle mbr = connectedComponent.MinBoundingRect;
                    Rectangle rectangle = new Rectangle()
                    {
                        Stroke = new SolidColorBrush(Colors.Blue),
                        StrokeThickness = inkCanvas.StrokeThickness / 2,
                        Width = mbr.Width,
                        Height = mbr.Height,
                        Margin = new Thickness() { Top = mbr.Top, Left = mbr.Left }
                    };
                    inkCanvas.Children.Add(rectangle);

                    byte[] orgBytes = new byte[(int)(mbr.Width * mbr.Height)];
                    byte[] scaBytes = new byte[(int)Math.Pow(mbr.Size, 2)];
                    for (int y = 0; y < mbr.Height; y++)
                    {
                        for (int x = 0; x < mbr.Width; x++)
                        {
                            if (connectedComponent.Points.Exists(p => p.X == mbr.Left + x && p.Y == mbr.Top + y))
                            {
                                double row = (mbr.Top + y) * canvasWidth;
                                double col = mbr.Left + x;
                                byte val = canvasBytes[(int)(row + col)];

                                int orgIdx = (int)(y * mbr.Width) + x;
                                orgBytes[orgIdx] = val;

                                int scaIdx = (int)(((y + mbr.PadTop) * mbr.Size) + (x + mbr.PadLeft));
                                scaBytes[scaIdx] = val;
                            }
                        }
                    }

                    byte[] orgRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(orgBytes, Colors.Black);
                    await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(orgRGBABytes, mbr.Width, mbr.Height, inkCanvas.Name + "_org");
                    connectedComponent.Pixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(orgBytes, mbr.Width, mbr.Height);

                    byte[] scaRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(scaBytes, Colors.Black);
                    scaRGBABytes = await ImageHelperRT.ScaleRGBAByteArrayAsync(scaRGBABytes, mbr.Size, mbr.Size, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    scaBytes = await ImageHelperRT.GetByteArrayFromRGBAByteArrayAsync(scaRGBABytes, inkCanvas.ForegroundColor);
                    await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight, inkCanvas.Name + "_sca");
                    connectedComponent.ScaledPixels = await ImageHelperRT.Get2DPixelArrayFromByteArrayAsync(scaBytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                }
            }
        }
    }
}
