using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blogio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Blogio.ViewModels;
using System.Diagnostics;

namespace Blogio.Controllers
{
    [Authorize(Roles = "Writer,Admin")]
    public class WriterController : Controller
    {
        private AppDbContext _context;
        private UserManager<User> _userManager;
        private IWebHostEnvironment _webHostEnvironment;

        public WriterController(AppDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page)
        {
            ViewBag.Title = "Blogio Writer";

            List<PostViewModel> posts = new List<PostViewModel>();
            foreach (var post in _context.Posts.ToList())
            {
                posts.Add(
                    new PostViewModel
                    {
                        PostId = post.PostId,
                        Title = post.Title,
                        Text = post.Text,
                        Picture = post.Picture,
                        Date = post.Date,
                        Rating = post.Rating,
                        Author = post.Author
                    }
                );
            }
            var model = new Blogio.PaginatedList
            {
                Posts = posts,
                PageSize = 15,
                CurrentPage = page == 0 ? 1 : page
            };
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel postVM)
        {
            User author = await _userManager.GetUserAsync(HttpContext.User);

            if (author.NickName != null)
            {
                postVM.Author = new Author { NickName = author.NickName };
                if (!_context.Authors.Any(a => a.NickName == author.NickName))
                {
                    _context.Authors.Add(postVM.Author);
                    _context.SaveChanges();
                }
            }

            if (postVM.PreviewPicture != null)
            {
                postVM.Picture = GetPicture(postVM.PreviewPicture);
            }
            if (postVM.Picture == "Wrong Format")
            {
                ModelState.AddModelError("", "Wrong Picture Format (.jpg, .jpeg and .png formats are allowed)");
            }

            if (ModelState.IsValid)
            {

                Post post = new Post
                {
                    Title = postVM.Title,
                    Text = postVM.Text,
                    Picture = postVM.Picture,
                    Date = DateTime.Now,
                    Rating = postVM.Rating,
                    Author = postVM.Author
                };
                _context.Posts.Add(post);
                if (postVM.Author != null)
                {
                    _context.Entry(post.Author).State = EntityState.Unchanged;
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Edit(long id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post != null)
            {
                return View(
                    new PostViewModel
                    {
                        PostId = post.PostId,
                        Title = post.Title,
                        Text = post.Text,
                        Picture = post.Picture,
                        Date = post.Date,
                        Rating = post.Rating,
                        Author = post.Author
                    }
                );
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel postVM)
        {
            if (ModelState.IsValid)
            {
                if (postVM.PreviewPicture != null)
                {
                    postVM.Picture = GetPicture(postVM.PreviewPicture);
                }
                Post post = new Post
                {
                    PostId = postVM.PostId,
                    Title = postVM.Title,
                    Text = postVM.Text,
                    Picture = postVM.Picture,
                    Date = postVM.Date,
                    Rating = postVM.Rating,
                    Author = postVM.Author
                };
                _context.Update(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post != null)
            {
                _context.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SearchResult(string searchQuery)
        {
            List<Post> dbposts = (from p in _context.Posts
                                  where (p.Title.Contains(searchQuery) || p.Text.Contains(searchQuery))
                                  select p).ToList();
            if (dbposts != null)
            {
                ViewBag.Title = $"'{searchQuery}' Search Results";

                List<PostViewModel> posts = new List<PostViewModel>();
                foreach (var post in dbposts)
                {
                    posts.Add(
                        new PostViewModel
                        {
                            PostId = post.PostId,
                            Title = post.Title,
                            Text = post.Text,
                            Picture = post.Picture,
                            Date = post.Date,
                            Rating = post.Rating,
                            Author = post.Author
                        }
                    );
                }

                var model = new Blogio.PaginatedList
                {
                    Posts = posts,
                    PageSize = 10,
                    CurrentPage = 1
                };

                return View(nameof(Index), model);
            }
            return NotFound();
        }

        private string? GetPicture(IFormFile picture)
        {
            if (picture != null)
            {
                string[] extensions = { ".jpeg", ".jpg", ".png" };
                foreach (var ext in extensions)
                {
                    if (Path.GetExtension(picture.FileName) == ext)
                    {
                        var downName = Path.Combine(_webHostEnvironment.WebRootPath + "/img/", Path.GetFileName(picture.FileName));
                        picture.CopyTo(new FileStream(downName, FileMode.Create));
                        return Path.Combine("/img/", Path.GetFileName(picture.FileName));
                    }
                }
                return "Wrong Format";
            }
            return null;
        }
    }
}