using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
    }
}
