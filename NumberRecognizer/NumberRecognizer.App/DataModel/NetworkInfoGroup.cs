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
    using System.Windows.Input;
    using NumberRecognizer.App.Common;
    using NumberRecognizer.App.View;
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
            this.InitializeProperties();
            this.InitializeCommands();
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
        public ObservableCollection<NetworkInfoRT> Networks { get; set; }

        /// <summary>
        /// Gets the click title command.
        /// </summary>
        /// <value>
        /// The click title command.
        /// </value>
        public ICommand ClickTitleCommand { get; private set; }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            this.ClickTitleCommand = new RelayCommand(this.TitleClicked);
        }

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void InitializeProperties()
        {
            this.Networks = new ObservableCollection<NetworkInfoRT>();
        }

        /// <summary>
        /// Titles the clicked.
        /// </summary>
        private void TitleClicked()
        {
            App.RootFrame.Navigate(typeof(GroupDetailPage), this);
        }
    }
}
