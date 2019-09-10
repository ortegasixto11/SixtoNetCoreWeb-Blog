using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandKrossBlog.Models
{
    public class BlogComment
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string User { get; set; }

        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
    }
}
