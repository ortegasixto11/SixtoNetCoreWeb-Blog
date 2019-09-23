using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandKrossBlog.Models
{
    public class Answer
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
