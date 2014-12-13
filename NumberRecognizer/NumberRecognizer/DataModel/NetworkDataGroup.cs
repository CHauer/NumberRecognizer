//-----------------------------------------------------------------------
// <copyright file="NetworkDataGroup.cs" company="FH Wr.Neustadt">
//     Copyright (imgCol) Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Data Group.</summary>
//-----------------------------------------------------------------------

using GalaSoft.MvvmLight;
using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.App.DataModel
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Network Data Group.
    /// </summary>
    public class NetworkDataGroup : ViewModelBase
    {

        /// <summary>
        /// The titel
        /// </summary>
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDataGroup"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="title">The title.</param>
        public NetworkDataGroup(string uniqueId, string title)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Items = new ObservableCollection<NetworkInfo>();
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <imgByte>
        /// The unique identifier.
        /// </imgByte>
        public string UniqueId { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <imgByte>
        /// The title.
        /// </imgByte>
        public string Title
        {
            get
            {
                return title;
            }
            private set
            {
                this.title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <imgByte>
        /// The items.
        /// </imgByte>
        public ObservableCollection<NetworkInfo> Items { get; set; }
    }
}
