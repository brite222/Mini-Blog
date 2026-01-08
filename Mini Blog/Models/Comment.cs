using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string AuthorName { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int BlogPostId { get; set; }
        public BlogPost? BlogPost { get; set; }
    }
}
