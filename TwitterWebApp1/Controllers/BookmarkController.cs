using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterWebApp1.Data;
using TwitterWebApp1.Models;

namespace TwitterWebApp1.Controllers
{
    public class BookmarkController : BaseController
    {
        public BookmarkController(ILogger<BaseController> logger, AppDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(logger, context, userManager, signInManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            //Check if user is logged in
            var loggedInUser = userManager.GetUserAsync(User).Result;

            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public ActionResult FetchPosts(int skipPost)
        {
            const int fetchCount = 10;

            var loggedInUser = userManager.GetUserAsync(User).Result;
            var userId = loggedInUser!.Id;

            IEnumerable<PostViewModel> bookmarkedPosts = context.Bookmarks
                .Where(b => b.User.Id == userId)
                .Select(b => b.Post)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((skipPost - 1) * fetchCount)
                .Take(fetchCount)
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

            var p = bookmarkedPosts.ToList();


            return PartialView("_PostsPartial", bookmarkedPosts);
        }
    }
}