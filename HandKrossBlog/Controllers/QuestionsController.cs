using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HandKrossBlog.Data;
using HandKrossBlog.Helpers;
using HandKrossBlog.Models;
using HandKrossBlog.Services;
using HandKrossBlog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HandKrossBlog.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IViewRenderService _viewRenderService;

        public const int NUMBER_OF_ITEMS_PER_REQUEST = 5;

        public QuestionsController(ApplicationDbContext context, IViewRenderService viewRenderService)
        {
            _context = context;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var question = await _context.Questions.Include(x => x.Answers).FirstOrDefaultAsync(x => x.Id == id);
            if (question == null) return NotFound();
            return View(question);
        }

        //[HttpGet]
        //[Route("/{user}")]
        //public IActionResult Index(string user)
        //{
        //    return View();
        //}

        public async Task<IActionResult> Home()
        {
            ViewModels.QuestionHomeViewModel questions = await GetQuestionsPaginated(null, null);
            return View(questions);
        }

        public async Task<IActionResult> GetPartialView_QuestionsPaginated(bool ShowNavbarPagination, int? PageIndex, int? TotalItems, string Query = null)
        {
            try
            {
                QuestionHomeViewModel viewModel = await GetQuestionsPaginated(PageIndex, TotalItems, Query);
                viewModel.ShowNavbarPagination = ShowNavbarPagination;
                var viewHtmlPaginationQuestions = viewModel.Questions.Count > 0 ? _viewRenderService.RenderToStringAsync("_PaginationQuestions", viewModel).Result : "";
                int totalNumberQuestions = await GetTotalNumberQuestions();
                return new JsonResult(new { Response = "true", ViewPaginationQuestions = viewHtmlPaginationQuestions, TotalNumberQuestions = totalNumberQuestions });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "false", Error = ex.Message });
            }
        }

        private async Task<int> GetTotalNumberQuestions()
        {
            int numberQuestions = 0;
            numberQuestions = await _context.Questions.CountAsync();
            return numberQuestions;
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
                var questions = _context.Questions.Include(x => x.Answers).WhereAll_AND(words.Select(w => (Expression<Func<Question, bool>>)(x => EF.Functions.Like(x.Text, "%" + w + "%"))).ToArray()).ToList();
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

        private async Task<QuestionHomeViewModel> GetQuestionsPaginated(int? PageIndex, int? TotalItems, string Query = null)
        {
            PageIndex = PageIndex == 0 ? 1 : PageIndex;
            if (TotalItems > 0) PageIndex = (TotalItems / NUMBER_OF_ITEMS_PER_REQUEST) + 1;

            QuestionHomeViewModel viewModel = new QuestionHomeViewModel();
            IQueryable<Question> questions = null;

            if (Query == null)
            {
                questions = _context.Questions.Include(x => x.Answers).OrderByDescending(x => x.DateCreated).AsNoTracking();
            }
            else
            {
                var words = Query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                questions = _context.Questions
                    .Include(x => x.Answers)
                    .OrderByDescending(x => x.DateCreated)
                    .WhereAll_AND(words.Select(w => (Expression<Func<Question, bool>>)(x => EF.Functions.Like(x.Text, "%" + w + "%"))).ToArray())
                    .AsNoTracking();
            }

            viewModel.Questions = await PaginatedList<Question>.CreateAsync(questions, PageIndex ?? 1, NUMBER_OF_ITEMS_PER_REQUEST);

            return viewModel;
        }

        [ActionName("SendAnswer")]
        public async Task<IActionResult> SendAnswerAsync(string Answer, Guid QuestionId)
        {
            var answer = new Answer {
                Id = Guid.NewGuid(),
                QuestionId = QuestionId,
                CreatedBy = "Anónimo",
                DateCreated = DateTime.Now,
                Text = Answer
            };

            try
            {
                _context.Answers.Add(answer);
                await _context.SaveChangesAsync();
                var viewHtmlAnswers = await GetPartialView_AnswersAsync(QuestionId);

                return new JsonResult(new { Response = "true", ViewHtmlAnswers = viewHtmlAnswers });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "Error", Error = ex.Message });
            }
        }

        private async Task<string> GetPartialView_AnswersAsync(Guid QuestionId)
        {
            try
            {
                var question = await _context.Questions.Include(x => x.Answers).FirstOrDefaultAsync(x => x.Id == QuestionId);
                if (question == null)
                {
                    return "No existe la Question";
                }
                else
                {
                    var viewHtml = await _viewRenderService.RenderToStringAsync("_Answers", question);
                    return viewHtml;
                }
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        [ActionName("SendPositivePointAnswer")]
        public async Task<IActionResult> SendPositivePointAnswerAsync(Guid AnswerId, Guid QuestionId)
        {
            try
            {
                var answer = await _context.Answers.FirstOrDefaultAsync(x => x.Id == AnswerId);
                answer.PositivePoints += 1;

                _context.Answers.Update(answer);
                await _context.SaveChangesAsync();
                var viewHtmlAnswers = await GetPartialView_AnswersAsync(QuestionId);

                return new JsonResult(new { Response = "true", ViewHtmlAnswers = viewHtmlAnswers });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "Error", Error = ex.Message });
            }
        }

        [ActionName("DeleteAnswer")]
        public async Task<IActionResult> DeleteAnswerAsync(Guid QuestionId, Guid AnswerId)
        {
            try
            {
                var answer = await _context.Answers.FirstOrDefaultAsync(x => x.Id == AnswerId);
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
                var viewHtmlAnswers = await GetPartialView_AnswersAsync(QuestionId);

                return new JsonResult(new { Response = "true", ViewHtmlAnswers = viewHtmlAnswers });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "Error", Error = ex.Message });
            }
        }

        [ActionName("UpdateAnswer")]
        public async Task<IActionResult> UpdateAnswerAsync(Guid QuestionId, Guid AnswerId, string Answer)
        {
            try
            {
                var answer = await _context.Answers.FirstOrDefaultAsync(x => x.Id == AnswerId);
                answer.Text = Answer;
                _context.Answers.Update(answer);
                await _context.SaveChangesAsync();
                var viewHtmlAnswers = await GetPartialView_AnswersAsync(QuestionId);
                return new JsonResult(new { Response = "true", ViewHtmlAnswers = viewHtmlAnswers });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "Error", Error = ex.Message });
            }
        }









    }
}