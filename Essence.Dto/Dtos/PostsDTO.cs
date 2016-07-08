using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essence.Dto
{
    public class PostsDTO
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}
