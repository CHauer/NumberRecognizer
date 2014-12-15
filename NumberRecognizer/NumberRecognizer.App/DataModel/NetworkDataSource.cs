//-----------------------------------------------------------------------
// <copyright file="NetworkDataSource.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Network Data Source.</summary>
//-----------------------------------------------------------------------

// The data model defined by this storageFile serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace NumberRecognizer.App.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Data.Json;
    using Windows.Storage;

    /// <summary>
    /// Network Data Source.
    /// </summary>
    public sealed class NetworkDataSource
    {
        /// <summary>
        /// The network data source.
        /// </summary>
        private static NetworkDataSource networkDataSource = new NetworkDataSource();

        /// <summary>
        /// The groups.
        /// </summary>
        private ObservableCollection<NetworkDataGroup> groups = new ObservableCollection<NetworkDataGroup>();

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public ObservableCollection<NetworkDataGroup> Groups
        {
            get { return this.groups; }
        }

        /// <summary>
        /// Gets the groups asynchronous.
        /// </summary>
        /// <returns>The groups.</returns>
        public static async Task<IEnumerable<NetworkDataGroup>> GetGroupsAsync()
        {
            await networkDataSource.GetNetworkDataAsync();

            return networkDataSource.Groups;
        }

        /// <summary>
        /// Gets the group asynchronous.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <returns>The groups.</returns>
        public static async Task<NetworkDataGroup> GetGroupAsync(string uniqueId)
        {
            await networkDataSource.GetNetworkDataAsync();
            //// Simple linear search is acceptable for small data sets
            var matches = networkDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                return matches.First();
            }

            return null;
        }

        /// <summary>
        /// Gets the item asynchronous.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <returns>The item async.</returns>
        public static async Task<NetworkDataItem> GetItemAsync(string uniqueId)
        {
            await networkDataSource.GetNetworkDataAsync();
            //// Simple linear search is acceptable for small data sets
            var matches = networkDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                return matches.First();
            }

            return null;
        }

        /// <summary>
        /// Gets the network data asynchronous.
        /// </summary>
        /// <returns>
        /// The network data.
        /// </returns>
        private async Task GetNetworkDataAsync()
        {
            if (this.groups.Count != 0)
            {
                return;
            }

            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                NetworkDataGroup group = new NetworkDataGroup(groupObject["UniqueId"].GetString(), groupObject["Title"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new NetworkDataItem(itemObject["UniqueId"].GetString(), itemObject["Title"].GetString()));
                }

                this.Groups.Add(group);
            }
        }
    }
}