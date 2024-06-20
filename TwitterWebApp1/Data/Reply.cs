using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitterWebApp1.Data;

public class Reply
{
    public int PostId { get; set; }
    public Post Post { get; set; }

    public int ReplyPostId { get; set; }
    public Post ReplyPost { get; set; }
}