using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace OcrTestApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ProcessImage_ButtonClick(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = new Bitmap(txtBitmapPath.Text);

            Bitmap bitmapleft = ImageToBlackWhite(bitmap);
           // Bitmap bitmapright = ImageToBlackWhite(bitmap);

            BitmapSource grayScale = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               bitmapleft.GetHbitmap(),
               IntPtr.Zero,
               System.Windows.Int32Rect.Empty,
               BitmapSizeOptions.FromWidthAndHeight(bitmapleft.Width, bitmapleft.Height));

            BitmapViewerLeft.Source = grayScale;


            var pixels = GetPixelsFromBitmap(bitmapleft);
        }

        private static double[,] GetPixelsFromBitmap(Bitmap bitmap)
        {
            double[,] pixelValues = new double[bitmap.Width, bitmap.Height];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(i, j);

                    if (pixel == System.Drawing.Color.FromArgb(255, 0, 0, 0))
                    {
                        pixelValues[i, j] = 1.0;
                    }
                    else
                    {
                        pixelValues[i, j] = 0;
                    }
                }
            }

            return pixelValues;
        }

        private static double[,] GetPixelsFromImagePath(string path)
        {
            double[,] pixelValues;

            using (Bitmap bitmap = new Bitmap(path))
            {
                pixelValues = GetPixelsFromBitmap(bitmap);
            }

            return pixelValues;
        }


        private Bitmap ImageToBlackWhite(Bitmap SourceImage)
        {
            using (Graphics gr = Graphics.FromImage(SourceImage)) // SourceImage is a Bitmap object
            {
                var gray_matrix = new float[][] { 
                new float[] { 0.299f, 0.299f, 0.299f, 0, 0 }, 
                new float[] { 0.587f, 0.587f, 0.587f, 0, 0 }, 
                new float[] { 0.114f, 0.114f, 0.114f, 0, 0 }, 
                new float[] { 0,      0,      0,      1, 0 }, 
                new float[] { 0,      0,      0,      0, 1 } 
            };

            
                var ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(gray_matrix));
                ia.SetThreshold((float) 0.8); // Change this threshold as needed
                var rc = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
                gr.DrawImage(SourceImage, rc, 0, 0, SourceImage.Width, SourceImage.Height, GraphicsUnit.Pixel, ia);
            }

            return SourceImage;
        }


        //public Bitmap ImageToBlackWhite(Bitmap original)
        //{

        //    Bitmap output = new Bitmap(original.Width, original.Height);

        //    for (int i = 0; i < original.Width; i++)
        //    {

        //        for (int j = 0; j < original.Height; j++)
        //        {

        //            System.Drawing.Color c = original.GetPixel(i, j);

        //            int average = ((c.R + c.B + c.G) / 3);

        //            if (average > 127)
        //                output.SetPixel(i, j, System.Drawing.Color.Black);

        //            else
        //                output.SetPixel(i, j, System.Drawing.Color.White);

        //        }
        //    }

        //    return output;

        //}

    }
}
