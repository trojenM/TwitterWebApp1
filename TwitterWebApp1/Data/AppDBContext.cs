using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace TwitterWebApp1.Data
{
    public class AppDBContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Reply>().HasKey(r => new { r.PostId, r.ReplyPostId });
            builder.Entity<Like>().HasKey(l => new { l.UserId, l.PostId });
            builder.Entity<Bookmark>().HasKey(b => new { b.UserId, b.PostId });
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Reply> Replies { get; set; }

        public DbSet<Like> Likes { get; set; }

        public DbSet<Bookmark> Bookmarks { get; set; }
    }
}