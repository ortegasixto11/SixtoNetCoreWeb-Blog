using HandKrossBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandKrossBlog.ViewModels
{
    public class BlogPostsIndex
    {
        public PaginatedList<BlogPost> BlogPosts { get; set; }
        public BlogPost LastBlogPost { get; set; }
        public bool ShowNavbarPagination { get; set; } = true;
    }
}
