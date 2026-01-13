using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        // Which post was liked
        [Required]
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; } = null!;

        // Who liked
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
