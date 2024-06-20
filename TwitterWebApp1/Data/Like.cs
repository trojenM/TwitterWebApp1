using System.ComponentModel.DataAnnotations;
using TwitterWebApp1.Data;

public class Like
{
    public int UserId { get; set; }
    public AppUser User { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}