using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; } = null!;

        // 🔑 Identity user
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public string AuthorName { get; set; } = "Anonymous";

    }

}

