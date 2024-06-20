using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using TwitterWebApp1.Data;
using TwitterWebApp1.Models;
using TwitterWebApp1.Models.Profile;

namespace TwitterWebApp1.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProfileController(ILogger<BaseController> logger, AppDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment webHostEnvironment) : base(logger, context, userManager, signInManager)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        // View profile page.
        [HttpGet]
        public IActionResult Index(string username)
        {
            var user = context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return RedirectToAction("Index", "Home");

            var loggedInUser = userManager.GetUserAsync(User).Result;

            var vm = new ProfileViewModel
            {
                UserName = user.UserName!,
                Name = user.Name,
                Bio = user.Bio ?? string.Empty,
                Location = user.Location ?? string.Empty,
                ProfileImage = user.ProfileImage ?? "default-pp.png",
                JoinedDate = user.CreatedAt.ToString("dd/MM/yyyy"),
                IsCurrentUser = loggedInUser != null && loggedInUser.Id == user.Id,
            };

            return View(vm);
        }

        // Update profile.
        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel vm)
        {
            var loggedInUser = userManager.GetUserAsync(User).Result;

            if (loggedInUser == null)
                return RedirectToAction("Login", "Account");

            loggedInUser.Name = vm.Name;
            loggedInUser.Bio = vm.Bio;
            loggedInUser.Location = vm.Location;
            if (vm.ProfileImageFile != null)
            {
                if (!string.IsNullOrEmpty(loggedInUser.ProfileImage))
                {
                    var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, "images", loggedInUser.ProfileImage);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                loggedInUser.ProfileImage = UploadImageFile(vm.ProfileImageFile);
            }

            var updateResult = userManager.UpdateAsync(loggedInUser).Result;

            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile.");
                return View(vm);
            }

            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Profile", new { username = loggedInUser.UserName });
        }

        // Show single post.
        [HttpGet]
        public IActionResult Post(int postHashCode)
        {
            //get post from database
            var post = context.Posts
                .Include(p => p.Author)
                .FirstOrDefault(p => p.Hash == postHashCode);

            if (post == null)
                return RedirectToAction("Index", "Home");


            var vm = new PostViewModel
            {
                UserName = post.Author.UserName!,
                Name = post.Author.Name,
                AuthorProfileImage = post.Author.ProfileImage ?? "default-pp.png",
                Description = post.Description,
                PostImage = post.PostImage ?? string.Empty,
                CreatedAt = post.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                Hash = post.Hash,
                LikesCount = post.LikesCount,
                RepliesCount = post.RepliesCount,
                BookmarksCount = post.BookmarksCount,
            };

            return View(vm);
        }

        // You can only reply a post from here.
        [HttpPost]
        public async Task<IActionResult> ReplyPost(PostViewModel vm)
        {
            var post = context.Posts
                .FirstOrDefault(p => p.Hash == vm.Hash);

            if (post == null)
                return RedirectToAction("Index", "Home");

            var loggedInUser = userManager.GetUserAsync(User).Result;

            if (loggedInUser == null)
                return RedirectToAction("Login", "Account");

            var replyPost = new Post
            {
                Description = vm.Description,
                CreatedAt = DateTime.Now,
                Author = loggedInUser,
                Hash = Guid.NewGuid().GetHashCode(),
            };

            var reply = new Reply
            {
                Post = post,
                ReplyPost = replyPost,
            };

            post.RepliesCount += 1;
            context.Posts.Add(replyPost);
            context.Replies.Add(reply);
            await context.SaveChangesAsync();

            return RedirectToAction("Post", "Profile", new { postHashCode = vm.Hash });
        }

        // Fetches reply posts of a post.
        [HttpGet]
        public IActionResult FetchReplyPosts(int postHashCode, int skipReplyPost)
        {
            const int fetchCount = 10;

            var post = context.Posts.FirstOrDefault(p => p.Hash == postHashCode);

            IEnumerable<PostViewModel> replyPosts = context.Replies
                .Where(r => r.Post == post)
                .Select(r => r.ReplyPost)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((skipReplyPost - 1) * fetchCount)
                .Take(fetchCount)
                .Select(p => new PostViewModel
                {
                    UserName = p.Author.UserName!,
                    Name = p.Author.Name,
                    AuthorProfileImage = p.Author.ProfileImage ?? "default-pp.png",
                    Description = p.Description,
                    PostImage = p.PostImage ?? string.Empty,
                    CreatedAt = p.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    Hash = p.Hash,
                    LikesCount = p.LikesCount,
                    RepliesCount = p.RepliesCount,
                    BookmarksCount = p.BookmarksCount,
                });

            return PartialView("_PostsPartial", replyPosts);
        }


        // Fetches profile user specific posts.
        [HttpGet]
        public IActionResult FetchPosts(string username, int skipPost)
        {
            const int fetchCount = 10;

            var user = context.Users.FirstOrDefault(u => u.UserName == username);
            var userId = user!.Id;

            IEnumerable<Post> replyPostsOfUser = context.Replies
                .Where(r => r.ReplyPost.Author.Id == userId)
                .Select(r => r.ReplyPost);

            IEnumerable<PostViewModel> postsOfUser = context.Posts
                .Where(p => p.Author.Id == userId)
                .Except(replyPostsOfUser)
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

            return PartialView("_PostsPartial", postsOfUser);
        }

        // Like post.
        [HttpPost]
        public async Task<IActionResult> Like(int hash)
        {
            var loggedInUser = await userManager.GetUserAsync(User);

            if (loggedInUser == null)
                return RedirectToAction("Login", "Account");

            var post = await context.Posts.FirstOrDefaultAsync(p => p.Hash == hash);

            if (post == null)
                return RedirectToAction("Index", "Home");

            var existingLike = await context.Likes.FirstOrDefaultAsync(l => l.User == loggedInUser && l.Post == post);

            if (existingLike != null)
            {
                context.Likes.Remove(existingLike);
                post.LikesCount -= 1;
            }
            else
            {
                var newLike = new Like
                {
                    User = loggedInUser,
                    Post = post,
                };

                context.Likes.Add(newLike);
                post.LikesCount += 1;
            }

            await context.SaveChangesAsync();
            return new JsonResult(new { likesCount = post.LikesCount });
        }

        [HttpPost]
        public async Task<IActionResult> Bookmark(int hash)
        {
            var loggedInUser = await userManager.GetUserAsync(User);

            if (loggedInUser == null)
                return RedirectToAction("Login", "Account");

            var post = await context.Posts.FirstOrDefaultAsync(p => p.Hash == hash);

            if (post == null)
                return RedirectToAction("Index", "Home");

            var existingBookmark = await context.Bookmarks.FirstOrDefaultAsync(b => b.User == loggedInUser && b.Post == post);

            if (existingBookmark != null)
            {
                context.Bookmarks.Remove(existingBookmark);
                post.BookmarksCount -= 1;
            }
            else
            {
                var newBookmark = new Bookmark
                {
                    User = loggedInUser,
                    Post = post,
                };

                context.Bookmarks.Add(newBookmark);
                post.BookmarksCount += 1;
            }

            await context.SaveChangesAsync();
            return new JsonResult(new { bookmarksCount = post.BookmarksCount });
        }

        private string? UploadImageFile(IFormFile? file)
        {
            if (file == null)
                return null;

            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images");
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

        private string? SaveImageFile(IFormFile? file)
        {
            if (file == null)
                return null;

            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images");
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadDir, fileName);

            using (var image = Image.Load(file.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(512, 512));
                image.Save(filePath);
            }

            return fileName;
        }
    }
}