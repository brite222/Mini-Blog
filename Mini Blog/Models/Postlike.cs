using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}