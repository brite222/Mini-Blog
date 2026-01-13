using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniBlog.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}
