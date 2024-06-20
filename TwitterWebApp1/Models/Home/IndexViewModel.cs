namespace TwitterWebApp1.Models.Home
{
    public class IndexViewModel
    {
        public CreatePostBoxViewModel? CreatePostBox { get; set; }

        public IEnumerable<PostViewModel>? Posts { get; set; }
    }
}