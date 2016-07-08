using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Mapping;
using Essence.Dto;

namespace Essence.DataProviders.NHibernate.Maps
{
    public class EpisodiesRatingMap : PersistenceMapping<EpisodiesRatingDTO>
    {
        public EpisodiesRatingMap()
            : base()
        {
            Table("EpisodiesRating");
            Not.LazyLoad();
            Map(x => x.imdbID);
            Map(x => x.Rating);
        }
    }
}

