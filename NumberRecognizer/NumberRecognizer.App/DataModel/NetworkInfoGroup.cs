//-----------------------------------------------------------------------
// <copyright file="NetworkInfoGroup.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Info Group.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.DataModel
{
    using System.Collections.ObjectModel;
    using NumberRecognizer.Cloud.Contract.Data;
    using PropertyChanged;

    /// <summary>
    /// Network Info Group.
    /// </summary>
    [ImplementPropertyChanged]
    public class NetworkInfoGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInfoGroup"/> class.
        /// </summary>
        public NetworkInfoGroup()
        {
            this.Networks = new ObservableCollection<NetworkInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInfoGroup"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="title">The title.</param>
        public NetworkInfoGroup(string uniqueId, string title)
            : this()
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
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the networks.
        /// </summary>
        /// <value>
        /// The networks.
        /// </value>
        public ObservableCollection<NetworkInfo> Networks { get; set; }
    }
}
