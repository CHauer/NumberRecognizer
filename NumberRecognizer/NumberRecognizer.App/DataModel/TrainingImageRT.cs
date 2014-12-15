//-----------------------------------------------------------------------
// <copyright file="TrainingImageRT.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Training Image RT.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.DataModel
{
    using NumberRecognizer.App.NumberRecognizerService;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Training Image RT.
    /// </summary>
    public class TrainingImageRT
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingImageRT"/> class.
        /// </summary>
        public TrainingImageRT()
        {
            this.Image = new TrainingImage();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingImageRT"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        public TrainingImageRT(TrainingImage image)
            : this()
        {
            this.Image = image;
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public TrainingImage Image { get; set; }

        /// <summary>
        /// Gets or sets the bitmap.
        /// </summary>
        /// <value>
        /// The bitmap.
        /// </value>
        public WriteableBitmap Bitmap { get; set; }
    }
}
