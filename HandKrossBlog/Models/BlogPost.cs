using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandKrossBlog.Models
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public string Fullname { get; set; }
        public string AvatarUrl { get; set; }
        public string Specialty_customized { get; set; }

        public ICollection<BlogComment> BlogPostComments { get; set; }
        public ICollection<BlogVisit> BlogPostVisits { get; set; }

    }
}
