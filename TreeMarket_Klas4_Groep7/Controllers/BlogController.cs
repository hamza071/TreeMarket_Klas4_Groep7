//using Microsoft.AspNetCore.Mvc;
//using TreeMarket_Klas4_Groep7.Data;
//using System.Linq;

//namespace TreeMarket_Klas4_Groep7.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class BlogController : ControllerBase
//    {
//        private readonly BloggingContext _context;

//        public BlogController(BloggingContext context)
//        {
//            _context = context;
//        }

//        // GET: api/blog
//        [HttpGet]
//        public IActionResult GetAllProducts()
//        {
//            var producten = _context.Producten.ToList();
//            return Ok(producten);
//        }

//        // POST: api/blog
//        [HttpPost]
//        public IActionResult AddProduct(Product product)
//        {
//            _context.Producten.Add(product);
//            _context.SaveChanges();
//            return CreatedAtAction(nameof(GetAllProducts), new { id = product.ID }, product);
//        }

//        // PUT: api/blog/5
//        [HttpPut("{id}")]
//        public IActionResult UpdateProduct(int id, Product updated)
//        {
//            var existing = _context.Producten.Find(id);
//            if (existing == null)
//                return NotFound();

//            // velden bijwerken
//            existing.Foto = updated.Foto;
//            existing.Artikelenkenmerken = updated.Artikelenkenmerken;
//            existing.Hoeveelheid = updated.Hoeveelheid;
//            existing.MinimumPrijs = updated.MinimumPrijs;
//            existing.LeverancierID = updated.LeverancierID;
//            existing.Dagdatum = updated.Dagdatum;

//            _context.SaveChanges();
//            return NoContent();
//        }

//        // DELETE: api/blog/5
//        [HttpDelete("{id}")]
//        public IActionResult DeleteProduct(int id)
//        {
//            var product = _context.Producten.Find(id);
//            if (product == null)
//                return NotFound();

//            _context.Producten.Remove(product);
//            _context.SaveChanges();
//            return NoContent();
//        }
//    }
//}

//dsd



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

        [HttpPut("{id}")]
        public IActionResult UpdateBlog(int id, Blog updatedBlog)
        {
            var blog = _context.Blogs.Find(id);
            if (blog == null) return NotFound();

            blog.Url = updatedBlog.Url;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBlog(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog == null) return NotFound();

            _context.Blogs.Remove(blog);
            _context.SaveChanges();
            return NoContent();
        }
    }
}