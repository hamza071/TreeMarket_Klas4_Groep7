using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

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

        // GET: api/Product/vandaag
        [HttpGet("vandaag")]
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                var today = DateTime.UtcNow.Date;

                var producten = await _context.Product
                    .Where(p => p.Dagdatum.Date == today)
                    .Select(p => new ProductMetVeilingmeesterDto
                    {
                        ProductId = p.ProductId,
                        Naam = p.ProductNaam,
                        Varieteit = p.Varieteit,
                        Omschrijving = p.Omschrijving,
                        Hoeveelheid = p.Hoeveelheid,
                        MinimumPrijs = p.MinimumPrijs,
                        Foto = p.Foto,
                        Status = "pending",
                        LeverancierNaam = p.Leverancier != null ? p.Leverancier.Bedrijf : null
                    })
                    .ToListAsync();

                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // GET: api/Product/leverancier
        [HttpGet("leverancier")]
        public async Task<IActionResult> GetMetLeverancier()
        {
            try
            {
                var producten = await _context.Product
                    .Include(p => p.Leverancier)
                    .Select(p => new ProductMetVeilingmeesterDto
                    {
                        ProductId = p.ProductId,
                        Naam = p.ProductNaam,
                        Varieteit = p.Varieteit,
                        Omschrijving = p.Omschrijving,
                        Hoeveelheid = p.Hoeveelheid,
                        MinimumPrijs = p.MinimumPrijs,
                        Foto = p.Foto,
                        Status = "pending",
                        LeverancierNaam = p.Leverancier != null ? p.Leverancier.Bedrijf : null
                    })
                    .ToListAsync();

                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // POST: api/Product/CreateProduct
        [HttpPost("CreateProduct")]
        [Authorize]
        public async Task<IActionResult> PostProduct([FromForm] ProductUploadDto productDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Je bent niet ingelogd.");

            try
            {
                // Foto uploaden
                string fotoUrl;
                if (productDto.Foto != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(productDto.Foto.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productDto.Foto.CopyToAsync(stream);
                    }

                    fotoUrl = "/images/" + uniqueFileName;
                }
                else
                {
                    fotoUrl = "/images/default.png";
                }

                // Leverancier ophalen
                var leverancier = await _context.Leverancier.FirstOrDefaultAsync(l => l.Id == userId);
                var isAdmin = User.IsInRole("Admin");
                string leverancierId = leverancier?.Id ?? (isAdmin ? "admin-01" : null);

                if (leverancierId == null)
                    return BadRequest("Er bestaat geen Leverancier-profiel voor deze gebruiker.");

                // Product aanmaken
                var product = new Product
                {
                    ProductNaam = productDto.ProductNaam ?? "",
                    Varieteit = productDto.Varieteit ?? "",
                    Omschrijving = productDto.Omschrijving ?? "",
                    Hoeveelheid = productDto.Hoeveelheid,
                    MinimumPrijs = productDto.MinimumPrijs,
                    Dagdatum = DateTime.UtcNow,
                    LeverancierID = leverancierId,
                    Foto = fotoUrl
                };

                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                // Return correct DTO voor frontend
                var productDtoResponse = new ProductMetVeilingmeesterDto
                {
                    ProductId = product.ProductId,
                    Naam = product.ProductNaam,
                    Varieteit = product.Varieteit,
                    Omschrijving = product.Omschrijving,
                    Hoeveelheid = product.Hoeveelheid,
                    MinimumPrijs = product.MinimumPrijs,
                    Foto = product.Foto,
                    Status = "pending",
                    LeverancierNaam = leverancier?.Bedrijf ?? (isAdmin ? "Admin" : null)
                };

                return Ok(new
                {
                    message = "Product succesvol aangemaakt!",
                    product = productDtoResponse
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "Databasefout.", error = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Serverfout.", error = ex.Message });
            }
        }
    }
}