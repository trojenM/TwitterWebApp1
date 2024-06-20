using TwitterWebApp1.Data;

public class Bookmark
{
    public int UserId { get; set; }
    public AppUser User { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}