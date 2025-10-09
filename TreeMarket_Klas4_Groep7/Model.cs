using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TreeMarket_Klas4_Groep7;

public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions<BloggingContext> options) : base(options) { }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();
}

public class Blog
{
    public int BlogId { get; set; }
    public required string Url { get; set; }
    public List<Post> Posts { get; set; } = new();
}

public class Post
{
    public int PostId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; } = default!;
}