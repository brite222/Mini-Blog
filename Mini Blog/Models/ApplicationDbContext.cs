using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniBlog.Models;

namespace MiniBlog.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BlogPostTag>()
                .HasKey(bt => new { bt.BlogPostId, bt.TagId });
        }

        // OPTIONAL: seed blog data (categories & tags)
        public static void SeedData(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Technology" },
                    new Category { Name = "Business" },
                    new Category { Name = "Lifestyle" },
                    new Category { Name = "Sports" }
                );
            }

            if (!context.Tags.Any())
            {
                context.Tags.AddRange(
                    new Tag { Name = "ASP.NET" },
                    new Tag { Name = "C#" },
                    new Tag { Name = "Web Development" },
                    new Tag { Name = "Tutorial" }
                );
            }

            context.SaveChanges();
        }
    }
}
