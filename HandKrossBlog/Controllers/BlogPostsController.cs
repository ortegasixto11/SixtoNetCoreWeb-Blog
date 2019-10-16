using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HandKrossBlog.Data;
using HandKrossBlog.Models;
using HandKrossBlog.Services;
using Microsoft.AspNetCore.Hosting;
using HandKrossBlog.ViewModels;
using HandKrossBlog.Helpers;
using System.Linq.Expressions;

namespace HandKrossBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IViewRenderService _viewRenderService;

        public const int NUMBER_OF_ITEMS_PER_REQUEST = 5;


        public BlogPostsController(ApplicationDbContext context, IViewRenderService viewRenderService)
        {
            _context = context;
            _viewRenderService = viewRenderService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index(int? PageIndex)
        {
            BlogPostsIndex viewModelIndex = await GetBlogPostsPaginated(true, PageIndex, null);
            return View(viewModelIndex);
        }

        public async Task<IActionResult> Home(int? PageIndex)
        {
            BlogPostsIndex viewModelIndex = await GetBlogPostsPaginated(false, PageIndex, null);
            viewModelIndex.ShowNavbarPagination = false; // Oculto la navegacion de la paginacion
            return View(viewModelIndex);
        }

        public async Task<IActionResult> GetPartialView_SectionPostPaginated(bool RegisterPostVisit, bool ShowNavbarPagination, int? PageIndex, int? TotalItems, string Query = null)
        {
            try
            {
                BlogPostsIndex viewModelIndex = await GetBlogPostsPaginated(RegisterPostVisit, PageIndex, TotalItems, Query);
                viewModelIndex.ShowNavbarPagination = ShowNavbarPagination;
                var viewHtmlPaginationPosts = viewModelIndex.BlogPosts.Count > 0 ? _viewRenderService.RenderToStringAsync("_PaginationBlogPosts", viewModelIndex).Result : "";
                var viewHtmlPost = RegisterPostVisit ? _viewRenderService.RenderToStringAsync("_BlogPost", viewModelIndex.LastBlogPost).Result : "";
                int totalPostnumber = await GetTotalPostNumber();
                return new JsonResult(new { Response = "true", ViewPaginationPosts = viewHtmlPaginationPosts, ViewPostActive = viewHtmlPost, TotalPostnumber = totalPostnumber });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "false", Error = ex.Message });
            }
        }

        private async Task<BlogPostsIndex> GetBlogPostsPaginated(bool RegisterPostVisit, int? PageIndex, int? TotalItems, string Query = null)
        {
            PageIndex = PageIndex == 0 ? 1 : PageIndex;
            if(TotalItems > 0) PageIndex = (TotalItems / NUMBER_OF_ITEMS_PER_REQUEST) + 1;

            BlogPostsIndex viewModelIndex = new BlogPostsIndex();
            IQueryable<BlogPost> blogPosts = null;

            if (Query == null)
            {
                blogPosts = _context.Posts
                                    .Include(x => x.BlogPostComments)
                                    .Include(x => x.BlogPostVisits)
                                    .OrderByDescending(x => x.DateCreated)
                                    .AsNoTracking();
            }
            else
            {
                var words = Query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                blogPosts = _context.Posts
                                    .Include(x => x.BlogPostComments)
                                    .Include(x => x.BlogPostVisits)
                                    .OrderByDescending(x => x.DateCreated)
                                    .WhereAll_AND(words.Select(w => (Expression<Func<BlogPost, bool>>)(x => EF.Functions.Like(x.Title + x.Fullname + x.Specialty_customized, "%" + w + "%"))).ToArray())
                                    .AsNoTracking();
            }

            viewModelIndex.BlogPosts = await PaginatedList<BlogPost>.CreateAsync(blogPosts, PageIndex ?? 1, NUMBER_OF_ITEMS_PER_REQUEST);
            viewModelIndex.LastBlogPost = viewModelIndex.BlogPosts.Count > 0 ? viewModelIndex.BlogPosts.First() : new BlogPost();

            if (RegisterPostVisit)
            {
                // Registro la visita del Post
                this.RegisterPostVisit(viewModelIndex.LastBlogPost.Id);
            }
            return viewModelIndex;
        }

        private async Task<int> GetTotalPostNumber()
        {
            int numberPosts = 0;
            numberPosts = await _context.Posts.CountAsync();
            return numberPosts;
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var blogPost = await _context.Posts.Include(x => x.BlogPostComments).FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null) return NotFound();
            // Registro la visita del Post
            RegisterPostVisit(blogPost.Id);
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Text,CreatedBy")] BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Title))
            {
                ViewData["ErrorMessage"] = "Ingrese un título";
                return View(blogPost);
            }
            if (string.IsNullOrEmpty(blogPost.Text))
            {
                ViewData["ErrorMessage"] = "Ingrese el texto del Post";
                return View(blogPost);
            }
            if (string.IsNullOrEmpty(blogPost.Fullname))
            {
                blogPost.Fullname = "Anónimo";
            }
            if (string.IsNullOrEmpty(blogPost.AvatarUrl))
            {
                blogPost.Fullname = "http://placehold.it/50x50";
            }
            if (string.IsNullOrEmpty(blogPost.Specialty_customized))
            {
                blogPost.Fullname = "Medico Cirujano";
            }

            if (ModelState.IsValid)
            {
                blogPost.Id = Guid.NewGuid();
                blogPost.DateCreated = DateTime.Now;
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var blogPost = await _context.Posts.FindAsync(id);
            if (blogPost == null) return NotFound();
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Text,DateCreated,CreatedBy")] BlogPost blogPost)
        {
            if (id != blogPost.Id) return NotFound();

            if (string.IsNullOrEmpty(blogPost.Title))
            {
                ViewData["ErrorMessage"] = "Ingrese un título";
                return View(blogPost);
            }
            if (string.IsNullOrEmpty(blogPost.Text))
            {
                ViewData["ErrorMessage"] = "Ingrese el texto del Post";
                return View(blogPost);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var blogPost = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null) return NotFound();
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var blogPost = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SaveComment(string Comment, string User, Guid PostId)
        {

            if (string.IsNullOrEmpty(User)) User = "Anónimo";

            var comment = new BlogComment {
                Id = Guid.NewGuid(),
                BlogPostId = PostId,
                Date = DateTime.Now,
                User = User,
                Text = Comment
            };

            try
            {
                _context.Comments.Add(comment);
                _context.SaveChanges();

                // Consulto el numero de comentarios
                var numberOfComments = _context.Comments.Where(x => x.BlogPostId == PostId).Select(x => x.Id).ToList().Count();
                // Obtengo la vista de los comentarios
                var viewHtml = GetPartialView_Comments(PostId);

                return new JsonResult(new { Response = "true", NumberOfComments = numberOfComments, ViewComments = viewHtml });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "Error", Error = ex.Message });
            }
        }

        public IActionResult DeleteComment(Guid CommentId, Guid PostId)
        {
            // Busco el Comment del Post
            var comment = _context.Comments.Where(x => x.Id == CommentId && x.BlogPostId == PostId).FirstOrDefault();
            if (comment == null)
            {
                return new JsonResult(new { Response = "false", Error = "No existe el comentario" });
            }
            else
            {
                // Elimino el comentario
                _context.Comments.Remove(comment);
                _context.SaveChanges();

                // Consulto el numero de comentarios
                var numberOfComments = _context.Comments.Where(x => x.BlogPostId == PostId).Select(x => x.Id).ToList().Count();
                // Obtengo la vista de los comentarios
                var viewHtml = GetPartialView_Comments(PostId);

                return new JsonResult(new { Response = "true", NumberOfComments = numberOfComments, ViewComments = viewHtml });
            }
        }

        public IActionResult EditComment(Guid CommentId, Guid PostId, string Comment)
        {
            // Busco el Comment del Post
            var comment = _context.Comments.Where(x => x.Id == CommentId && x.BlogPostId == PostId).FirstOrDefault();
            if (comment == null)
            {
                return new JsonResult(new { Response = "false", Error = "No existe el comentario" });
            }
            else
            {
                // Edito el comentario
                _context.Attach<BlogComment>(comment).State = EntityState.Modified;
                comment.Text = Comment;
                _context.Comments.Update(comment);
                _context.SaveChanges();

                // Consulto el numero de comentarios
                var numberOfComments = _context.Comments.Where(x => x.BlogPostId == PostId).Select(x => x.Id).ToList().Count();
                // Obtengo la vista de los comentarios
                var viewHtml = GetPartialView_Comments(PostId);

                return new JsonResult(new { Response = "true", NumberOfComments = numberOfComments, ViewComments = viewHtml });
            }
        }

        public IActionResult GetPartialView_Post(Guid PostId)
        {
            try
            {
                var post = _context.Posts.Where(x => x.Id == PostId).Include(x => x.BlogPostComments).Include(x => x.BlogPostVisits).FirstOrDefault();
                if (post == null)
                {
                    return new JsonResult(new { Response = "false", Error = "No existe el Post" });
                }
                else
                {
                    // Registro la visita del Post
                    RegisterPostVisit(post.Id);
                    var viewHtml = _viewRenderService.RenderToStringAsync("_BlogPost", post);
                    return new JsonResult(new { Response = "true", View = viewHtml.Result, NumberOfVisits = post.BlogPostVisits.Count() });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Response = "false", Error = ex.Message });
            }
        }

        private string GetPartialView_Comments(Guid PostId)
        {
            try
            {
                var post = _context.Posts.Where(x => x.Id == PostId).Include(x => x.BlogPostComments).FirstOrDefault();
                if (post == null) 
                {
                    return "No existe el Post";
                }
                else
                {
                    var viewHtml = _viewRenderService.RenderToStringAsync("_BlogPostComment", post);
                    return viewHtml.Result;
                }
            }
            catch (Exception ex)
            {
                return "Error " + ex.Message;
            }
        }

        private void RegisterPostVisit(Guid PostId)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress;
            var modelVisit = new BlogVisit
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                PostId = PostId,
                IpAddress = ipAddress.ToString()
            };

            _context.Visits.Add(modelVisit);
            _context.SaveChanges();

        }

        private bool BlogPostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }


    }
}
