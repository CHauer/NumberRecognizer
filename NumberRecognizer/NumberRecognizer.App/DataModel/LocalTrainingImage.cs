//-----------------------------------------------------------------------
// <copyright file="LocalTrainingImage.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Training Image RT.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.DataModel
{
    using NumberRecognizer.App.NumberRecognizerService;
    using NumberRecognizer.Cloud.Contract.Data;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Training Image RT.
    /// </summary>
    public class LocalTrainingImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTrainingImage"/> class.
        /// </summary>
        public LocalTrainingImage()
        {
            this.Image = new TrainingImage();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTrainingImage"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        public LocalTrainingImage(TrainingImage image)
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
