namespace TwitterWebApp1.Models
{
    public class PostViewModel
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string AuthorProfileImage { get; set; }

        public string Description { get; set; }

        public string PostImage { get; set; }

        public IEnumerable<PostViewModel> ReplyPosts { get; set; }

        public int LikesCount { get; set; }

        public int RepliesCount { get; set; }

        public int BookmarksCount { get; set; }

        public string CreatedAt { get; set; }

        public int Hash { get; set; }
    }
}