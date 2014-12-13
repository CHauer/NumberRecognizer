using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using NumberRecognizer.App.DataModel;
using NumberRecognizer.App.NumberRecognizerService;
using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.App.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupedNetworksViewModel : ViewModelBase
    {
        /// <summary>
        /// The groups
        /// </summary>
        private ObservableCollection<NetworkDataGroup> groups;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedNetworksViewModel"/> class.
        /// </summary>
        public GroupedNetworksViewModel()
        {
            LoadNetworks(); 
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads the networks.
        /// </summary>
        private async void LoadNetworks()
        {
            NumberRecognizerServiceClient serviceProxy = new NumberRecognizerServiceClient();
            IList<NetworkInfo> networks;

            try
            {
                networks = await serviceProxy.GetNetworksAsync();
            }
            catch
            { 
                return;
            }

            groups = new ObservableCollection<NetworkDataGroup>();

            foreach(string groupName in networks.Select(n => n.Username).Distinct())
            {
                var group = new NetworkDataGroup(groupName, groupName);
                foreach(var network in networks.Where(n => n.Username.Equals(group.Title)))
                {
                    group.Items.Add(network);
                }

                groups.Add(group);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <imgByte>
        /// The groups.
        /// </imgByte>
        public ObservableCollection<NetworkDataGroup> Groups
        {
            get { return this.groups; }
        }

        #endregion

        #region Commands

        #endregion
    }
}
