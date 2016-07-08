using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essence.Dto
{
    public class SearchDTO
    {
        public virtual IList<SerieDTO> Search { get; set; }
    }
}
