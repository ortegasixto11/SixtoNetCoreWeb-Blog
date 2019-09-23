using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HandKrossBlog.Data;
using HandKrossBlog.Helpers;
using HandKrossBlog.Models;
using HandKrossBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HandKrossBlog.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IViewRenderService _viewRenderService;

        public QuestionsController(ApplicationDbContext context, IViewRenderService viewRenderService)
        {
            _context = context;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/{user}")]
        public IActionResult Index(string user)
        {
            return View();
        }

        public IActionResult SaveQuestion(string Question)
        {
            var question = new Question {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.Now,
                CreatedBy = "Anónimo",
                Text = Question
            };

            try
            {
                _context.Questions.Add(question);
                _context.SaveChanges();

                return new JsonResult(new { Response = "true" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "error", Error = ex.Message });
            }
        }

        public IActionResult SearchResult(string Question)
        {
            try
            {
                var words = Question.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var questions = _context.Questions.WhereAll_AND(words.Select(w => (Expression<Func<Question, bool>>)(x => EF.Functions.Like(x.Text, "%" + w + "%"))).ToArray()).ToList();
                string view_searchResults = GetPartialView_SearchResults(questions);
                return new JsonResult(new { Response = "true", viewSearchResults = view_searchResults });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "error", Error = ex.Message });
            }
        }

        private string GetPartialView_SearchResults(List<Question> questions)
        {
            try
            {
                var viewHtml = _viewRenderService.RenderToStringAsync("_QuestionSearchResults", questions);
                return viewHtml.Result;
            }
            catch (Exception ex)
            {
                return "Error " + ex.Message;
            }
        }

    }
}