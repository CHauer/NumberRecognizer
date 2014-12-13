using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Mutzl.MvvmLight;
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

        /// <summary>
        /// The current network
        /// </summary>
        private NetworkInfo currentNetwork;

        /// <summary>
        /// The command create network
        /// </summary>
        private ICommand cmdCreateNetwork;

        /// <summary>
        /// The command delete network
        /// </summary>
        private DependentRelayCommand cmdDeleteNetwork;

        /// <summary>
        /// The command refresh networks
        /// </summary>
        private ICommand cmdRefreshNetworks;

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

            var groupTrained = new NetworkDataGroup("Trained", "Trained");
            foreach (var network in networks.Where(n => n.Calculated))
            {
                groupTrained.Items.Add(network);
            }
            Groups.Add(groupTrained);

            var groupNotTrained = new NetworkDataGroup("Not Trained", "Not Trained");
            foreach (var network in networks.Where(n => !n.Calculated))
            {
                groupNotTrained.Items.Add(network);
            }

            Groups.Add(groupNotTrained);
        }

        /// <summary>
        /// Loads the commands.
        /// </summary>
        private void LoadCommands()
        {
            CreateNetwork = new RelayCommand(() => App.Frame.Navigate(typeof(CreateNetworkPage)));
            RefreshNetworks = new RelayCommand(LoadNetworks);
            DeleteNetwork = new DependentRelayCommand(ExecuteDeleteNetwork, CanExecuteDeleteNetwork, this, () => SelectedNetwork);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current selected network.
        /// </summary>
        /// <value>
        /// The current selected network.
        /// </value>
        public NetworkInfo SelectedNetwork
        {
            get
            {
                return currentNetwork;
            }
            set
            {
                currentNetwork = value;
                RaisePropertyChanged(() => SelectedNetwork);
            }
        }

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

        /// <summary>
        /// Gets or sets the delete network.
        /// </summary>
        /// <value>
        /// The delete network.
        /// </value>
        public DependentRelayCommand DeleteNetwork
        {
            get
            {
                return cmdDeleteNetwork;
            }
            set
            {
                cmdDeleteNetwork = value;
                RaisePropertyChanged(() => DeleteNetwork);
            }
        }

        /// <summary>
        /// Gets or sets the refresh networks.
        /// </summary>
        /// <value>
        /// The refresh networks.
        /// </value>
        public ICommand RefreshNetworks
        {
            get
            {
                return cmdRefreshNetworks;
            }
            set
            {
                cmdRefreshNetworks = value;
                RaisePropertyChanged(() => RefreshNetworks);
            }
        }

        #endregion

        /// <summary>
        /// Executes the delete network.
        /// </summary>
        private void ExecuteDeleteNetwork()
        {
            NumberRecognizerServiceClient serviceProxy = new NumberRecognizerServiceClient();

            //serviceProxy.Delete
        }

        /// <summary>
        /// Determines whether this instance [can execute delete network].
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteDeleteNetwork()
        {
            return SelectedNetwork != null;
        }

    }
}
