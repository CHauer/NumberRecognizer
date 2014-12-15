//-----------------------------------------------------------------------
// <copyright file="TrainingImageGroup.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Training Image Group.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.DataModel
{
    using System.Collections.ObjectModel;
    using GalaSoft.MvvmLight;
    using PropertyChanged;

    /// <summary>
    /// Training Image Group.
    /// </summary>
    [ImplementPropertyChanged]
    public class TrainingImageGroup : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingImageGroup"/> class.
        /// </summary>
        public TrainingImageGroup()
        {
            this.Images = new ObservableCollection<TrainingImageRT>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingImageGroup"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="title">The title.</param>
        public TrainingImageGroup(string uniqueId, string title) : this()
        {
            this.UniqueId = uniqueId;
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        public ObservableCollection<TrainingImageRT> Images { get; set; }
    }
}
