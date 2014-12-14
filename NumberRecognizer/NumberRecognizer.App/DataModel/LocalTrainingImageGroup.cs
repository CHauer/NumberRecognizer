using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace NumberRecognizer.App.DataModel
{
    public class LocalTrainingImageGroup : ViewModelBase
    {
         /// <summary>
        /// The titel
        /// </summary>
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDataGroup"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="title">The title.</param>
        public LocalTrainingImageGroup(string uniqueId, string title)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Items = new ObservableCollection<LocalTrainingImage>();
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <imgByte>
        /// The unique identifier.
        /// </imgByte>
        public string UniqueId { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <imgByte>
        /// The title.
        /// </imgByte>
        public string Title
        {
            get
            {
                return title;
            }
            private set
            {
                this.title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <imgByte>
        /// The items.
        /// </imgByte>
        public ObservableCollection<LocalTrainingImage> Items { get; set; }
    }
}
