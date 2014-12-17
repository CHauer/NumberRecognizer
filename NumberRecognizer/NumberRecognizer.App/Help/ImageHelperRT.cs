//-----------------------------------------------------------------------
// <copyright file="ImageHelperRT.cs" company="FH Wr.Neustadt">
//     Copyright (c) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>ImageHelper for RT.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics;

namespace NumberRecognizer.App.Help
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Image Helper.
    /// </summary>
    public static class ImageHelperRT
    {
        /// <summary>
        /// The image height.
        /// </summary>
        public const double ImageHeight = 16;

        /// <summary>
        /// The image width.
        /// </summary>
        public const double ImageWidth = 16;

        /// <summary>
        /// The RGBA constant.
        /// </summary>
        private const int RGBA = 4;

        /// <summary>
        /// Gets the byte array from red, green, blue, alpha byte array.
        /// </summary>
        /// <param name="rgbaByteArray">The red, green, blue, alpha byte array.</param>
        /// <param name="foreground">The foreground.</param>
        /// <param name="background">The background.</param>
        /// <returns>The byte array from red, green, blue, alpha byte array.</returns>
        public static byte[] GetByteArrayFromRGBAByteArray(byte[] rgbaByteArray, Color foreground, Color background)
        {
            byte[] byteArray = new byte[rgbaByteArray.Length / RGBA];
            int counterbefore = 0;
            int counter = 0; 

            for (int i = 0; i < rgbaByteArray.Length; i++)
            {
                Color color = new Color();
                color.R = rgbaByteArray[i];
                color.G = rgbaByteArray[++i];
                color.B = rgbaByteArray[++i];
                color.A = rgbaByteArray[++i];

                if (color.R == foreground.R && color.R == foreground.G && color.R == foreground.B && color.A != 0)
                {
                    //byteArray[i / RGBA] = foreground.A;
                    counter++;
                }

                if (color.R == foreground.R && color.R == foreground.G && color.R == foreground.B && color.A > 100)
                {
                    byteArray[i / RGBA] = foreground.A;
                    counterbefore++;
                }
            }

            Debug.WriteLine("Before:{0} After:{1}", counterbefore, counter);

            return byteArray;
        }

        /// <summary>
        /// Gets the red, green, blue, alpha byte array from byte array.
        /// </summary>
        /// <param name="byteArray">The byte array.</param>
        /// <param name="foreground">The foreground.</param>
        /// <returns>
        /// The red, green, blue, alpha byte array from byte array.
        /// </returns>
        public static byte[] GetRGBAByteArrayFromByteArrayAsync(byte[] byteArray, Color foreground)
        {
            byte[] rgbaByteArray = new byte[byteArray.Length * RGBA];

            for (int i = 0; i < byteArray.Length; i++)
            {
                if (byteArray[i] > 0)
                {
                    int r = i * RGBA;
                    rgbaByteArray[r] = foreground.R;
                    rgbaByteArray[++r] = foreground.G;
                    rgbaByteArray[++r] = foreground.B;
                    rgbaByteArray[++r] = foreground.A;
                }
            }

            return rgbaByteArray;
        }

        /// <summary>
        /// Gets the pixels from byte array.
        /// </summary>
        /// <param name="byteArray">The byte array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The pixels from byte array.</returns>
        public static double[,] Get2DPixelArrayFromByteArray(byte[] byteArray, double width, double height)
        {
            double[,] pixelArray2D = new double[(int)height, (int)width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int z = (int)(width * y) + x;
                    if (byteArray[z] != 0)
                    {
                        pixelArray2D[y, x] = byteArray[z] / 255;
                    }
                }
            }

            return pixelArray2D;
        }

        /// <summary>
        /// Gets the byte array from pixel 2d array.
        /// </summary>
        /// <param name="pixelArray2D">The pixel 2d array.</param>
        /// <returns>The byte array from pixel 2d array.</returns>
        public static byte[] GetByteArrayFrom2DPixelArrayAsync(double[,] pixelArray2D)
        {
            int length = pixelArray2D.Length;
            int height = pixelArray2D.GetLength(0);
            int width = pixelArray2D.GetLength(1);
            byte[] byteArray = new byte[length];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int z = (width * y) + x;
                    if (pixelArray2D[y, x] != 0)
                    {
                        byteArray[z] = (byte)(pixelArray2D[y, x] * 255);
                    }
                }
            }

            return byteArray;
        }

        /// <summary>
        /// Gets the observable collection from 2D pixel array.
        /// </summary>
        /// <param name="pixelArray">The pixel array.</param>
        /// <returns>The observable collection from 2D pixel array.</returns>
        public static ObservableCollection<double> GetObservableCollectionFrom2DPixelArray(double[,] pixelArray)
        {
            ObservableCollection<double> observableCollection = new ObservableCollection<double>();

            for (int y = 0; y < pixelArray.GetLength(1); y++)
            {
                for (int x = 0; x < pixelArray.GetLength(0); x++)
                {
                    observableCollection.Add(pixelArray[y, x]);
                }
            }

            return observableCollection;
        }

        /// <summary>
        /// Saves the red, green, blue, alpha byte array as bitmap image asynchronous.
        /// </summary>
        /// <param name="rgbaByteArray">The red, green, blue, alpha byte array.</param>
        /// <param name="width">The width parameter.</param>
        /// <param name="height">The height parameter.</param>
        /// <param name="name">The name parameter.</param>
        /// <returns>
        /// The red, green, blue, alpha byte array as bitmap image asynchronous.
        /// </returns>
        public static async Task SaveRGBAByteArrayAsBitmapImageAsync(byte[] rgbaByteArray, double width, double height, string name)
        {
            var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(name + ".png", CreationCollisionOption.GenerateUniqueName);

            using (var randomAccessStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
                bitmapEncoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)width, (uint)height, 96d, 96d, rgbaByteArray);
                bitmapEncoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;
                await bitmapEncoder.FlushAsync();
            }
        }

        /// <summary>
        /// Saves the red, green, blue, alpha byte array as memory stream.
        /// </summary>
        /// <param name="rgbaByteArray">The red, green, blue, alpha byte array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        /// The red, green, blue, alpha byte array as memory stream.
        /// </returns>
        public static async Task<InMemoryRandomAccessStream> SaveRGBAByteArrayAsMemoryStream(byte[] rgbaByteArray, double width, double height)
        {
            var inMemoryRandomAccessStream = new InMemoryRandomAccessStream();

            var bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, inMemoryRandomAccessStream);

            bitmapEncoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)width, (uint)height, 96d, 96d, rgbaByteArray);
            bitmapEncoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;

            await bitmapEncoder.FlushAsync();

            return inMemoryRandomAccessStream;
        }

        /// <summary>
        /// Scales the red, green, blue, alpha byte array asynchronous.
        /// </summary>
        /// <param name="rgbaByteArray">The red, green, blue, alpha byte array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="scaledWidth">Width of the scaled.</param>
        /// <param name="scaledHeight">Height of the scaled.</param>
        /// <returns>The scaled red, green, blue, alpha byte array asynchronous.</returns>
        public static async Task<byte[]> ScaleRGBAByteArrayAsync(byte[] rgbaByteArray, double width, double height, double scaledWidth, double scaledHeight)
        {
            var inMemoryRandomAccessStream = new InMemoryRandomAccessStream();

            var bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, inMemoryRandomAccessStream);

            bitmapEncoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)width, (uint)height, 96d, 96d, rgbaByteArray);
            bitmapEncoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;

            if (height != scaledHeight || width != scaledWidth)
            {
                bitmapEncoder.BitmapTransform.ScaledWidth = (uint)scaledWidth;
                bitmapEncoder.BitmapTransform.ScaledHeight = (uint)scaledHeight;
            }

            await bitmapEncoder.FlushAsync();

            var bitmap = new WriteableBitmap((int)scaledWidth, (int)scaledHeight);
            await bitmap.SetSourceAsync(inMemoryRandomAccessStream);
            return bitmap.PixelBuffer.ToArray();
        }
    }
}