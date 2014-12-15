//-----------------------------------------------------------------------
// <copyright file="NetworkDataItem.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Data Item.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.DataModel
{
    /// <summary>
    /// Network Data Item.
    /// </summary>
    public class NetworkDataItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDataItem"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="title">The title.</param>
        public NetworkDataItem(string uniqueId, string title)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
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
    }
}
