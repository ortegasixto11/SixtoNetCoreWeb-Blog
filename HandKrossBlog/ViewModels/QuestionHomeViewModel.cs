using HandKrossBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandKrossBlog.ViewModels
{
    public class QuestionHomeViewModel
    {
        public PaginatedList<Question> Questions { get; set; }
        public bool ShowNavbarPagination { get; set; } = true;
    }
}
