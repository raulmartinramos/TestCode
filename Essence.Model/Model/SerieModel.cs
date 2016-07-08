using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Essence.Dto;


namespace Essence.Model
{
    public class SerieModel: PersistableBase, INotifyPropertyChanged
    {

        private string _imdbID;
        private string _seriesID;
        private string _Title;
        private string _Year;
        private string _Rated;
        private string _Released;
        private string _Season;
        private string _Episode;
        private string _Runtime;
        private string _Genre;
        private string _Director;
        private string _Writer;
        private string _Actors;
        private string _Plot;
        private string _Language;
        private string _Country;
        private string _Awards;
        private string _Poster;
        private string _Metascore;
        private string _imdbRating;
        private string _imdbVotes;
        private string _Type;

        private bool _Favorite;
        private int _Rating;



        public string imdbID { get { return _imdbID; } set { _imdbID = value; NotifyPropertyChanged("imdbID"); } }
        public string seriesID { get { return _seriesID; } set { _seriesID = value; NotifyPropertyChanged("seriesID"); } }
        public string Title { get { return _Title; } set { _Title = value; NotifyPropertyChanged("Title"); } }
        public string Year { get { return _Year; } set { _Year = value; NotifyPropertyChanged("Year"); } }
        public string Rated { get { return _Rated; } set { _Rated = value; NotifyPropertyChanged("Rated"); } }
        public string Released { get { return _Released; } set { _Released = value; NotifyPropertyChanged("Released"); } }
        public string Season { get { return _Season; } set { _Season = value; NotifyPropertyChanged("Season"); } }
        public string Episode { get { return _Episode; } set { _Episode = value; NotifyPropertyChanged("Episode"); } }
        public string Runtime { get { return _Runtime; } set { _Runtime = value; NotifyPropertyChanged("Runtime"); } }
        public string Genre { get { return _Genre; } set { _Genre = value; NotifyPropertyChanged("Genre"); } }
        public string Director { get { return _Director; } set { _Director = value; NotifyPropertyChanged("Director"); } }
        public string Writer { get { return _Writer; } set { _Writer = value; NotifyPropertyChanged("Writer"); } }
        public string Actors { get { return _Actors; } set { _Actors = value; NotifyPropertyChanged("Actors"); } }
        public string Plot { get { return _Plot; } set { _Plot = value; NotifyPropertyChanged("Plot"); } }
        public string Language { get { return _Language; } set { _Language = value; NotifyPropertyChanged("Language"); } }
        public string Country { get { return _Country; } set { _Country = value; NotifyPropertyChanged("Country"); } }
        public string Awards { get { return _Awards; } set { _Awards = value; NotifyPropertyChanged("Awards"); } }
        public string Poster { get { return _Poster; } set { _Poster = value; NotifyPropertyChanged("Poster"); } }
        public string Metascore { get { return _Metascore; } set { _Metascore = value; NotifyPropertyChanged("Metascore"); } }
        public string imdbRating { get { return _imdbRating; } set { _imdbRating = value; NotifyPropertyChanged("imdbRating"); } }
        public string imdbVotes { get { return _imdbVotes; } set { _imdbVotes = value; NotifyPropertyChanged("imdbVotes"); } }
        public string Type { get { return _Type; } set { _Type = value; NotifyPropertyChanged("Type"); } }

        public bool Favorite { get { return _Favorite; } set { _Favorite = value; NotifyPropertyChanged("Favorite"); } }
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
