using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

        private ICommand cmdCreateNetwork;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedNetworksViewModel"/> class.
        /// </summary>
        public GroupedNetworksViewModel()
        {
            LoadNetworks();
            LoadCommands();
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

            Groups = new ObservableCollection<NetworkDataGroup>();

            foreach(string groupName in networks.Select(n => n.Username).Distinct())
            {
                var group = new NetworkDataGroup(groupName, groupName);
                foreach(var network in networks.Where(n => n.Username.Equals(group.Title)))
                {
                    group.Items.Add(network);
                }

                Groups.Add(group);
            }
        }

        /// <summary>
        /// Loads the commands.
        /// </summary>
        private void LoadCommands()
        {
            AddNetwork = new RelayCommand(() => App.Frame.Navigate(typeof(CreateNetworkPage)));
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
            get
            {
                return this.groups;
            }
            set
            {
                groups = value;
                RaisePropertyChanged(() => Groups);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the add network.
        /// </summary>
        /// <value>
        /// The add network.
        /// </value>
        public ICommand CreateNetwork
        {
            get
            {
                return cmdCreateNetwork;
            }
            set
            {
                cmdCreateNetwork = value;
                RaisePropertyChanged(() => CreateNetwork);
            }
        }

        #endregion
    }
}
