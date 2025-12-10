using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using TreeMarket_Klas4_Groep7.Services;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ApiContext _context;

        public ProductController(ProductService productService, ApiContext context)
        {
            _productService = productService;
            _context = context;
        }

        // Haal producten van vandaag op
        [HttpGet("vandaag")]
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                var producten = await _productService.GetProductenVanVandaag();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Product van vandaag kan niet getoond worden.", error = ex.Message });
            }
        }

        // Haal producten met Leverancier info op
        [HttpGet("leverancier")]
        public async Task<IActionResult> GetMetLeverancier()
        {
            try
            {
                var producten = await _productService.GetProductenMetLeverancier();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Product van de leverancier kan niet getoond worden.", error = ex.Message });
            }
        }

        // Voeg nieuw product toe of update bestaand product
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Artikelkenmerken))
                return BadRequest("Artikelkenmerken is verplicht.");

            if (product.Hoeveelheid <= 0)
                return BadRequest("Hoeveelheid moet groter zijn dan 0.");

            if (product.MinimumPrijs < 0)
                return BadRequest("MinimumPrijs mag niet negatief zijn.");

            if (product.Dagdatum.Date < DateTime.Today)
                return BadRequest("Dagdatum mag niet in het verleden liggen.");

            // Hier controleren we op lege string in plaats van <= 0
            if (string.IsNullOrWhiteSpace(product.LeverancierID))
                return BadRequest("LeverancierID is verplicht.");

            try
            {
                var result = await _productService.AddOrUpdateProduct(product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Product kan niet toegevoegd of geupdate worden.", error = ex.Message });
            }
        }

        // POST via DTO
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> PostProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = new Product
                {
                    Foto = productDto.Foto,
                    Artikelkenmerken = productDto.artikelkenmerken,
                    Hoeveelheid = productDto.Hoeveelheid,
                    MinimumPrijs = productDto.MinimumPrijs,
                    Dagdatum = DateTime.UtcNow,
                    LeverancierID = productDto.leverancierID // string
                };

                await _context.Product.AddAsync(product);
                await _context.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Product kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        // Upload product via form
        [HttpPost("upload")]
        public async Task<IActionResult> UploadProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? imagePath = null;

            if (dto.Image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                imagePath = "/images/" + fileName;
            }

            var product = new Product
            {
                Artikelkenmerken = dto.Variety ?? "",
                Hoeveelheid = dto.Quantity,
                MinimumPrijs = dto.MinPrice,
                Foto = imagePath ?? "",
                Dagdatum = DateTime.UtcNow,
                LeverancierID = dto.LeverancierID // string
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // Haal alle pending kavels op
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProducts()
        {
            try
            {
                var pending = await _context.Product
                    .Where(p => !_context.Veiling.Any(v => v.ProductID == p.ProductId))
                    .Select(p => new
                    {
                        code = p.ProductId,
                        name = p.Artikelkenmerken,
                        description = "",
                        lots = p.Hoeveelheid,
                        image = p.Foto,
                        status = "pending",
                        productID = p.ProductId,
                        minPrice = p.MinimumPrijs
                    })
                    .ToListAsync();

                return Ok(pending);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Kan pending kavels niet ophalen.", error = ex.Message });
            }
        }
    }
}