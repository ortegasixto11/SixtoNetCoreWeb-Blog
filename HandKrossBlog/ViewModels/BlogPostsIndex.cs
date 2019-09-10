using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandKrossBlog.ViewModels
{
    public class BlogPostsIndex
    {
        public IEnumerable<HandKrossBlog.Models.BlogPost> BlogPosts { get; set; }
        public HandKrossBlog.Models.BlogPost LastBlogPost { get; set; }
    }
}
