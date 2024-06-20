
using Microsoft.AspNetCore.Identity;

namespace TwitterWebApp1.Data
{
    public class AppUser : IdentityUser<int>
    {
        public override string? Email { get; set; }

        public override string? UserName { get; set; }

        public string Name { get; set; }

        public string? Bio { get; set; }

        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ProfileImage { get; set; }

        //public List<Post> LikedPosts { get; set; }
    }
}