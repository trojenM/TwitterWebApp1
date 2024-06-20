using System.ComponentModel.DataAnnotations;
using TwitterWebApp1.Data;

public class Post
{
    [Key]
    public int Id { get; set; }

    public int AuthorId { get; set; }
    public AppUser Author { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? PostImage { get; set; }

    public int LikesCount { get; set; }

    public int RepliesCount { get; set; }

    public int BookmarksCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public int Hash { get; set; }
}