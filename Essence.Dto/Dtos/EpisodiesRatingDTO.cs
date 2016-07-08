using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essence.Dto
{
    public class EpisodiesRatingDTO : PersistableBase
    {
        public string imdbID { get; set; }
        public int Rating { get; set; }
    }
}
