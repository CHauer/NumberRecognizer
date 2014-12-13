using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NumberRecognizer.Cloud.Contract.Data
{
    [DataContract]
    public class RecognitionImage
    {
        [DataMember]
        public double[] ImageData { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public int Width{ get; set; }

        public void TransformFrom2DArrayToImageData(double[,] array)
        {
            int index = 0;
            ImageData = new double[Height * Width];

            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    ImageData[index++] = array[h, w];
                }
            }
        }

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
