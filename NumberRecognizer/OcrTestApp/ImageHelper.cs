using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NumberRecognizer.Lib.Network;

namespace OcrTestApp
{

	public class ImageHelper
	{
		public static double[,] GetPixelsFromCanvas(InkCanvas canvas, int width, int height)
		{
			double[,] pixelValues;

			using (Bitmap bitmap = SaveInkCanvasToMemoryStream(canvas, width, height))
			{
				pixelValues = GetPixelsFromBitmap(bitmap);
			}

			return pixelValues;
		}

		public static ICollection<PatternTrainingImage> ReadTrainingData(string directoryPath)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

			ICollection<PatternTrainingImage> trainingData = new List<PatternTrainingImage>();

			foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
			{
				foreach (FileInfo fileInfo in directory.GetFiles().Where(x => x.Extension == ".png"))
				{
					trainingData.Add(new PatternTrainingImage()
					{
						RepresentingInformation = directory.Name,
						PixelValues = GetPixelsFromImagePath(fileInfo.FullName)
					});
				}
			}

			return trainingData;
		}

		public static void SaveInkCanvasToPng(InkCanvas canvas, string path, int width, int height)
		{
			Bitmap bitmap = SaveInkCanvasToMemoryStream(canvas, width, height);
			bitmap.Save(path);
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

		private static Bitmap SaveInkCanvasToMemoryStream(InkCanvas canvas, int width, int height)
		{
			// Save canvas with original size to memory stream
			VisualBrush sourceBrush = new VisualBrush(canvas);

			DrawingVisual drawingVisual = new DrawingVisual();
			DrawingContext drawingContext = drawingVisual.RenderOpen();

			using (drawingContext)
			{
				drawingContext.DrawRectangle(sourceBrush, null,
					new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(canvas.ActualWidth, canvas.ActualHeight)));
			}

			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d,
				96d, PixelFormats.Default);
			renderTargetBitmap.Render(drawingVisual);

			MemoryStream memoryStream = new MemoryStream();

			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
			encoder.Save(memoryStream);

			memoryStream.Seek(0, SeekOrigin.Begin);

			// Load original bitmap from memory stream
			Bitmap bitmap = new Bitmap(memoryStream);

			memoryStream.Close();

			// Create resized bitmap
			Bitmap resizedBitmap = new Bitmap(width, height);

			// Calculate bounding rectangle
			double[,] pixelsFromBitmap = GetPixelsFromBitmap(bitmap);

			int left = pixelsFromBitmap.GetLength(0) - 1;
			int top = pixelsFromBitmap.GetLength(1) - 1;
			int right = 0;
			int bottom = 0;

			for (int i = 0; i < pixelsFromBitmap.GetLength(0); i++)
			{
				for (int j = 0; j < pixelsFromBitmap.GetLength(1); j++)
				{
					if (pixelsFromBitmap[i, j] == 1.0)
					{
						left = Math.Min(left, i);
						top = Math.Min(top, j);
						right = Math.Max(right, i);
						bottom = Math.Max(bottom, j);
					}
				}
			}

			using (Graphics graphics = Graphics.FromImage(resizedBitmap))
			{
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

				// Calculate width / height ratio
				double ratio;

				if (right - left > bottom - top)
				{
					ratio = ((double)bottom - top) / ((double)right - left);
				}
				else
				{
					ratio = ((double)right - left) / ((double)bottom - top);
				}

				// Substract 10% Buffer from width / height
				int maxInnerWidth = (int)(width - ((double)2 * width / 10));
				int maxInnerHeight = (int)(height - ((double)2 * height / 10));

				int x = (int)((width / 2) - ((double)maxInnerWidth / 2));
				int y = (int)((height / 2) - ((double)maxInnerHeight / 2));

				int graphicsWidth = maxInnerWidth;
				int graphicsHeight = maxInnerHeight;

				// Modify x or y depending on ratio (maximum scale of 1.5)
				if (right - left > bottom - top)
				{
					y = Math.Max(y, (int)((height / 2) - (maxInnerHeight * (ratio * 1.5) / 2)));
					graphicsHeight = Math.Min(maxInnerHeight, (int)Math.Ceiling(maxInnerHeight * (ratio * 1.5)));
				}
				else
				{
					x = Math.Max(x, (int)((width / 2) - (maxInnerWidth * (ratio * 1.5) / 2)));
					graphicsWidth = Math.Min(maxInnerWidth, (int)Math.Ceiling(maxInnerWidth * (ratio * 1.5)));
				}

				// Draw bitmap
				graphics.DrawImage(bitmap, new Rectangle(x, y, graphicsWidth, graphicsHeight),
					new Rectangle(left, top, right - left, bottom - top), GraphicsUnit.Pixel);
			}

			return resizedBitmap;
		}
	}
}