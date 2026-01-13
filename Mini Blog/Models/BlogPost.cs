using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();

        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

        public string? ImageUrl { get; set; }
    }
}
