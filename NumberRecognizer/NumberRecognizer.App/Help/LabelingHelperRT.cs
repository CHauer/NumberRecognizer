//-----------------------------------------------------------------------
// <copyright file="LabelingHelperRT.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Labeling Helper for RT.</summary>
//-----------------------------------------------------------------------


using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NumberRecognizer.App.Help
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using NumberRecognition.Labeling;
    using NumberRecognizer.App.Control;
    using NumberRecognizer.App.NumberRecognizerService;    
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Labeling Helper for RT.
    /// </summary>
    public class LabelingHelperRT
    {
        /// <summary>
        /// Connected component labeling for canvas.
        /// </summary>
        /// <param name="inkCanvas">The ink canvas.</param>
        /// <returns>Connected Component Task for Ink Canvas.</returns>
        public static async Task ConnectedComponentLabelingForInkCanvasRT(InkCanvasRT inkCanvas)
        {
            inkCanvas.RefreshCanvas();
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(inkCanvas);
            
            await SaveVisualElementToFile(renderTargetBitmap, await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("testimage.png", CreationCollisionOption.ReplaceExisting));
            
            byte[] canvasRGBABytes = (await renderTargetBitmap.GetPixelsAsync()).ToArray();
            byte[] canvasBytes = ImageHelperRT.GetByteArrayFromRGBAByteArray(canvasRGBABytes, inkCanvas.ForegroundColor, inkCanvas.BackgroundColor);

            double canvasWidth = inkCanvas.ActualWidth;
            double canvasHeight = inkCanvas.ActualHeight;
            double[,] canvasPixels = ImageHelperRT.Get2DPixelArrayFromByteArray(canvasBytes, canvasWidth, canvasHeight);

            inkCanvas.Labeling = new ConnectedComponentLabeling();
            inkCanvas.Labeling.TwoPassLabeling(canvasPixels, 0.0);

            foreach (ConnectedComponent connectedComponent in inkCanvas.Labeling.ConnectedComponents)
            {
                MinimumBoundingRectangle mbr = connectedComponent.MinBoundingRect;
                Rectangle rectangle = new Rectangle()
                {
                    Stroke = new SolidColorBrush(Colors.OrangeRed),
                    StrokeThickness = 2.0,
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
                        if (connectedComponent.ComponentPoints.Exists(p => p.X == mbr.Left + x && p.Y == mbr.Top + y))
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

                connectedComponent.Pixels = ImageHelperRT.Get2DPixelArrayFromByteArray(orgBytes, mbr.Width, mbr.Height);
                connectedComponent.Bytes = orgBytes;

                try
                {
                    byte[] scaRGBABytes = ImageHelperRT.GetRGBAByteArrayFromByteArrayAsync(scaBytes, inkCanvas.ForegroundColor);
                    scaRGBABytes = await ImageHelperRT.ScaleRGBAByteArrayAsync(scaRGBABytes, mbr.Size, mbr.Size, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    scaBytes = ImageHelperRT.GetByteArrayFromRGBAByteArray(scaRGBABytes, inkCanvas.ForegroundColor, inkCanvas.BackgroundColor);
                    connectedComponent.ScaledPixels = ImageHelperRT.Get2DPixelArrayFromByteArray(scaBytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight);
                    connectedComponent.ScaledBytes = scaBytes;

                    await ImageHelperRT.SaveRGBAByteArrayAsBitmapImageAsync(scaRGBABytes, ImageHelperRT.ImageWidth, ImageHelperRT.ImageHeight, inkCanvas.Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        async static Task SaveVisualElementToFile(RenderTargetBitmap bitmap, StorageFile file)
        {

            var pixels = await bitmap.GetPixelsAsync();

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await
                    BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Ignore,
                                     (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight,
                                     96, 96, bytes);

                await encoder.FlushAsync();
            }
        }

    }
}
