using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandKrossBlog.Models
{
    public class BlogVisit
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string IpAddress { get; set; }

        public Guid PostId { get; set; }
        public BlogPost Post { get; set; }
    }
}
