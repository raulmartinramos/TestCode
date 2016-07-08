using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Mapping;
using Essence.Dto;

namespace Essence.DataProviders.NHibernate.Maps
{
    public class SeriesFavoritesMap : PersistenceMapping<SeriesFavoritesDTO>
    {
        public SeriesFavoritesMap()
            : base()
        {
            Table("SeriesFavorites");
            Not.LazyLoad();
            Map(x => x.imdbID);
            Map(x => x.seriesID);
            Map(x => x.Title);
            Map(x => x.Year);
            Map(x => x.Rated);
            Map(x => x.Released);
            Map(x => x.Season);
            Map(x => x.Episode);
            Map(x => x.Runtime);
            Map(x => x.Genre);
            Map(x => x.Director);
            Map(x => x.Writer);
            Map(x => x.Actors);
            Map(x => x.Plot);
            Map(x => x.Language);
            Map(x => x.Country);
            Map(x => x.Awards);
            Map(x => x.Poster);
            Map(x => x.Metascore);
            Map(x => x.imdbRating);
            Map(x => x.imdbVotes);
            Map(x => x.Type);
        }
    }
}

