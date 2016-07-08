using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Essence.Dto;

namespace Essence.Model
{
    public class EpisodiesRatingModel : PersistableBase, INotifyPropertyChanged
    {

        private string _imdbID;
        private int _Rating;

        public string imdbID { get { return _imdbID; } set { _imdbID = value; NotifyPropertyChanged("imdbID"); } }
        public int Rating { get { return _Rating; } set { _Rating = value; NotifyPropertyChanged("Rating"); } }

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
