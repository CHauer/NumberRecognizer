using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using NumberRecognizer.App.DataModel;

namespace NumberRecognizer.App.ViewModel
{
    public class ValidateTrainingDataViewModel : ViewModelBase
    {
        private ObservableCollection<LocalTrainingImageGroup> groups;

        public ValidateTrainingDataViewModel(Dictionary<int, List<LocalTrainingImage>> trainData)
        {
            LoadGroups(trainData);
        }

        private void LoadGroups(Dictionary<int, List<LocalTrainingImage>> trainData)
        {
            //Property Changed Raise
            Groups = new ObservableCollection<LocalTrainingImageGroup>();

            foreach (int i in trainData.Keys)
            {
                var group = new LocalTrainingImageGroup(i.ToString(), i.ToString());

                foreach (LocalTrainingImage image in trainData[i])
                {
                    group.Items.Add(image);
                }

                groups.Add(group);
            }
        }

        public ObservableCollection<LocalTrainingImageGroup> Groups
        {
            get
            {
                return groups;
            }
            set
            {
                groups = value;
                RaisePropertyChanged(() => Groups);
            }
        }


    }
}
