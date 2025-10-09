using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly BloggingContext _context;

        public BlogController(BloggingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllBlogs()
        {
            var blogs = _context.Blogs.ToList();
            return Ok(blogs);
        }

        [HttpPost]
        public IActionResult AddBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAllBlogs), new { id = blog.BlogId }, blog);
        }
    }
}