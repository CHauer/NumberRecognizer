using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class RecognitionImage
    {
        /// <summary>
        /// Gets or sets the image data.
        /// </summary>
        /// <value>
        /// The image data.
        /// </value>
        [DataMember]
        public double[] ImageData { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [DataMember]
        public int Width{ get; set; }

        /// <summary>
        /// Transforms the from2 d array to image data.
        /// </summary>
        /// <param name="array">The array.</param>
        public void TransformFrom2DArrayToImageData(double[,] array)
        {
            int index = 0;
            int height = array.GetLength(0);
            int width = array.GetLength(1);

            ImageData = new double[height * height];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    ImageData[index++] = array[h, w];
                }
            }
        }

        /// <summary>
        /// Transforms the to2 d array from image data.
        /// </summary>
        /// <returns></returns>
        public double[,] TransformTo2DArrayFromImageData()
        {
            int index = 0;
            double[,] array = new double[Height, Width];

            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    array[h, w] = ImageData[index++];
                }
            }

            return array;
        }
    }
}
