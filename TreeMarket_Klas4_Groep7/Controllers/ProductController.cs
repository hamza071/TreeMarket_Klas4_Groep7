using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApiContext _context;

        public ProductController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public JsonResult CreateProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                //Dit gaat een gebruiker aanmaken
                _context.Product.Add(product);
            }
            else
            {
                var gebruikerInDb = _context.Product.Find(product.ProductId);

                if (gebruikerInDb == null)
                {
                    return new JsonResult(NotFound());
                }

                gebruikerInDb = product;
            }

            _context.SaveChanges();

            return new JsonResult(Ok(product));

        }
    }
}
