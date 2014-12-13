using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumberRecognizer.App.DataModel;
using NumberRecognizer.Cloud.Contract.Data;

namespace NumberRecognizer.App.ViewModel
{
    public class GroupedNetworksDesignViewModel
    {
        /// <summary>
        /// The groups
        /// </summary>
        private ObservableCollection<NetworkDataGroup> groups;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedNetworksViewModel" /> class.
        /// </summary>
        public GroupedNetworksDesignViewModel()
        {
            LoadGroups();
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads the groups.
        /// </summary>
        private void LoadGroups()
        {
            groups = new ObservableCollection<NetworkDataGroup>()
            {
                new NetworkDataGroup("MZytek", "MZytek"){
                    Items = new ObservableCollection<NetworkInfo>(){
                        new NetworkInfo(){
                            Username = "MZytek",
                            NetworkFitness = 11.2,
                            NetworkId = 1,
                            NetworkName = "Test1",
                            Calculated = true,
                        },
                        new NetworkInfo(){
                            Username = "MZytek",
                            NetworkFitness = 67.2,
                            NetworkId = 1,
                            NetworkName = "Test2",
                            Calculated = true,
                        }
                    }
                },
                new NetworkDataGroup("CHauer", "CHauer"){
                    Items = new ObservableCollection<NetworkInfo>(){
                        new NetworkInfo(){
                            Username = "CHauer",
                            NetworkFitness = 54.3,
                            NetworkId = 1,
                            NetworkName = "NetworkTest1",
                            Calculated = true,
                        },
                        new NetworkInfo(){
                            Username = "CHauer",
                            NetworkFitness = 56.3,
                            NetworkId = 1,
                            NetworkName = "NetworkTest2",
                            Calculated = true,
                        }
                    }
                }
            };
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

    }
}
