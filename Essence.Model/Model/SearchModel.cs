using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace Essence.Model
{
    public class SearchModel : INotifyPropertyChanged
    {
        private IList<SerieModel> _Search;
        public IList<SerieModel> Search { get { return _Search; } set { _Search = value; NotifyPropertyChanged("Search"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
