using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blogio.Models;
using Microsoft.AspNetCore.Identity;
using Blogio.ViewModels;

namespace Blogio.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _context;
        private UserManager<User> _userManager;
        private IWebHostEnvironment webHostEnvironment;
        public HomeController(AppDbContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page)
        {
            ViewBag.Title = "Main Page Blogio";

            if (page == 0)
            {
                page = 1;
            }

            List<PostViewModel> posts = new List<PostViewModel>();
            foreach(var post in _context.Posts.Include(a => a.Author).ToList())            
            {
                posts.Add(
                    new PostViewModel {
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
                PageSize = 6,
                CurrentPage = page
            };

            return View(model);
        }

        public async Task<IActionResult> Details(long id)
        {
            var dbpost = _context.Posts.Include(p => p.Author).Include(p => p.Comments).FirstOrDefault(p => p.PostId == id);

            if (dbpost != null)
            {
                ViewBag.Title = $"{dbpost.Title} Details Blogio";
                var post = new PostViewModel
                {
                    PostId = dbpost.PostId,
                    Title = dbpost.Title,
                    Text = dbpost.Text,
                    Picture = dbpost.Picture,
                    Rating = dbpost.Rating,
                    Date = dbpost.Date,
                    Author = dbpost.Author,
                    Comments = dbpost.Comments
                };
                return View(post);
            }
            else
            {
                dbpost = _context.Posts.Include(p => p.Author).FirstOrDefault();
                if(dbpost == null)
                {
                    return NotFound();
                }

                ViewBag.Title = $"{dbpost.Title} Details Blogio";
                var post = new PostViewModel
                {
                    PostId = dbpost.PostId,
                    Title = dbpost.Title,
                    Text = dbpost.Text,
                    Picture = dbpost.Picture,
                    Rating = dbpost.Rating,
                    Date = dbpost.Date,
                    Author = dbpost.Author
                };
                return View(post);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Details([FromForm]PostViewModel postVM)
        {
            if(postVM.NewRating != null)
            {
                if(postVM.Rating != null)
                {
                    postVM.Rating = (int)(Math.Ceiling((double)(postVM.NewRating + postVM.Rating) / 2));
                }  
                else if(postVM.Rating == null)          
                {
                    postVM.Rating = postVM.NewRating;
                }

                Post post = new Post { PostId = postVM.PostId, Rating = postVM.Rating };

                _context.Posts.Attach(post);
                _context.Entry(post).Property(r => r.Rating).IsModified = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = ++post.PostId });
            }
            
            if(postVM.NewComment != null)
            {
                var comment = new Comment {
                    PostId = postVM.PostId,
                    Text = postVM.NewComment.Text,
                    Date = DateTime.Now,
                    UserName = User.Identity.Name
                };
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details));
            }
            return RedirectToAction(nameof(Details));            
        }

        public IActionResult Search(string searchQuery, int page)
        {
            List<Post> dbposts = (from p in _context.Posts
                                  where (p.Title.Contains(searchQuery) || p.Text.Contains(searchQuery))
                                  select p).ToList();
            if (dbposts != null)
            {
                ViewBag.Title = $"'{searchQuery}' Search Results";
                
                List<PostViewModel> posts = new List<PostViewModel>();
                foreach(var post in dbposts)            
                {
                    posts.Add(
                        new PostViewModel {
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
                    PageSize = 6,
                    CurrentPage = page
                };

                return View(model);
            }
            return NotFound();
        }

        public IActionResult AboutMe()
        {
            ViewBag.Title = "About Me";
            Post? post = _context.Posts.Where(p => p.Title == "About Me").FirstOrDefault();
            if(post == null)
            {
                return NotFound();
            }
            else
            {
                return View(new AboutMeViewModel { Title = post.Title, Picture = post.Picture, Text = post.Text });
            }            
        }  

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
            long postId = 0;
            if(comment != null)
            {
                postId = comment.PostId;
                _context.Remove(comment);
                await _context.SaveChangesAsync();
            }  
            return RedirectToAction(nameof(Details), new { id = postId });
        }
    }
}