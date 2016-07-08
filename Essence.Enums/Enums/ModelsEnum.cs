using System;
using System.Collections.Generic;
using System.Linq;

namespace Essence.Enums
{
    public class ModelsEnum
    {
        public enum TiposModels : int
        {
            Post = 1,
            Serie = 2,
            Search = 3,
            SeriesFavorites = 4,
            EpisodiesRating = 5,
            Espisodies=6,
        }

        public enum TiposMetodos : int
        {
            Get = 1,
            Post = 2,
            Put = 3,
            Delete = 3,
        }
    }
}
