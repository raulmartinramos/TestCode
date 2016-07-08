using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Http;
using Essence.Global;
using System.Collections.ObjectModel;
using Essence.Model;

namespace Essence.ViewModel
{
    public class GlobalDataFrameWork
    {
        private SeriesFavoritesCollection seriesFavorites;
        private EpisodiesRatingCollection episodiesRating;

        public GlobalDataFrameWork()
        {
            SeriesFavorites = new SeriesFavoritesCollection();
            episodiesRating = new EpisodiesRatingCollection();
        }


        public SeriesFavoritesCollection SeriesFavorites { get { return seriesFavorites; } set { if (value != null) { seriesFavorites = value; } } }
        public EpisodiesRatingCollection EpisodiesRating { get { return episodiesRating; } set { if (value != null) { episodiesRating = value; } } }
        
        

    }
}

