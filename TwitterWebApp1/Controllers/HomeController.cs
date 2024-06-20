using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterWebApp1.Data;
using TwitterWebApp1.Models;
using TwitterWebApp1.Models.Home;

namespace TwitterWebApp1.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(ILogger<BaseController> logger, AppDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment webHostEnvironment) : base(logger, context, userManager, signInManager)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = userManager.GetUserAsync(User).Result;

            if (user != null)
            {
                var vm = new IndexViewModel
                {
                    CreatePostBox = new CreatePostBoxViewModel
                    {
                        ProfileImage = user.ProfileImage ?? "default-pp.png",
                    }
                };

                return View(vm);
            }

            return RedirectToAction("Login", "Account");
        }

        // For creating post on feed.
        [HttpPost]
        public async Task<IActionResult> Post(IndexViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var loggedInUser = userManager.GetUserAsync(User).Result;

                if (loggedInUser != null)
                {
                    var post = new Post
                    {
                        Description = vm.CreatePostBox!.Description ?? string.Empty,
                        PostImage = UploadImageFile(vm.CreatePostBox.InputImageFile),
                        CreatedAt = DateTime.Now,
                        Author = loggedInUser,
                        Hash = Guid.NewGuid().GetHashCode(),
                    };

                    context.Posts.Add(post);
                    await context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "User not found.");
                return View(vm);
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult FetchPosts(int skipPost)
        {
            const int fetchCount = 10;

            IEnumerable<Post> replyPosts = context.Replies.Select(r => r.ReplyPost);

            IEnumerable<PostViewModel> posts = context.Posts
                .Except(replyPosts)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((skipPost - 1) * fetchCount)
                .Take(fetchCount)
                .Include(p => p.Author)
                .Select(p => new PostViewModel
                {
                    UserName = p.Author.UserName!,
                    Name = p.Author.Name,
                    AuthorProfileImage = p.Author.ProfileImage ?? "default-pp.png",
                    Description = p.Description,
                    PostImage = p.PostImage ?? string.Empty,
                    CreatedAt = p.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    LikesCount = p.LikesCount,
                    RepliesCount = p.RepliesCount,
                    BookmarksCount = p.BookmarksCount,
                    Hash = p.Hash,
                });

            return PartialView("_PostsPartial", posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string? UploadImageFile(IFormFile? file)
        {
            if (file == null)
                return null;

            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images/post");
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }
    }
}