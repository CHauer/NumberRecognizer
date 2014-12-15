//-----------------------------------------------------------------------
// <copyright file="NetworkDataGroup.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Data Group.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.DataModel
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Network Data Group.
    /// </summary>
    public class NetworkDataGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDataGroup"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="title">The title.</param>
        public NetworkDataGroup(string uniqueId, string title)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Items = new ObservableCollection<NetworkDataItem>();
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public string UniqueId { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<NetworkDataItem> Items { get; private set; }
    }
}
