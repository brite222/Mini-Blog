using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
