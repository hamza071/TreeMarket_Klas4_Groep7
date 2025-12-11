using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        private readonly IProductController _service;
        private readonly ApiContext _context; // ✅ Voeg context toe

        public ProductController(IProductController service, ApiContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("vandaag")]
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                var producten = await _service.GetProductenVanVandaagAsync();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }
        private async Task<List<ProductMetVeilingmeesterDto>> GetProductenVanVandaagAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Product
                .Where(p => p.Dagdatum.Date == today)
                .Select(p => new ProductMetVeilingmeesterDto
                {
                    ProductId = p.ProductId,                   // <- hier was het p.Id
                    Name = p.Artikelkenmerken ?? "Geen naam",
                    Description = "Omschrijving nog toevoegen",
                    Lots = p.Hoeveelheid,
                    MinimumPrijs = p.MinimumPrijs,
                    Image = p.Foto,
                    Status = "pending",
                    LeverancierNaam = p.Leverancier != null ? p.Leverancier.Bedrijf : null
                })
                .ToListAsync();
        }

        [HttpGet("leverancier")]
        public async Task<IActionResult> GetMetLeverancier()
        {
            try
            {
                var producten = await _service.GetProductenMetLeverancierAsync();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // ================= CREATE / UPDATE =================
        [HttpPost("CreateProduct")]
        [Authorize] // alleen ingelogde gebruikers mogen
        public async Task<IActionResult> PostProduct([FromForm] ProductUploadDto productDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("Je bent niet ingelogd.");

            try
            {
                // -------------------- Image Upload --------------------
                string fotoUrl;
                if (productDto.Image != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(productDto.Image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productDto.Image.CopyToAsync(stream);
                    }

                    fotoUrl = "/images/" + uniqueFileName;
                }
                else
                {
                    fotoUrl = "/images/default.png"; // default afbeelding
                }

                // -------------------- Leverancier ophalen --------------------
                var leverancier = await _context.Leverancier
                    .FirstOrDefaultAsync(l => l.Id == userId);

                var isAdmin = User.IsInRole("Admin");

                // -------------------- Dummy ID voor admin --------------------
                string leverancierId = leverancier?.Id ?? (isAdmin ? "admin-01" : null);

                if (leverancierId == null)
                {
                    return BadRequest("Er bestaat geen Leverancier-profiel voor deze gebruiker.");
                }

                // -------------------- Product aanmaken --------------------
                var product = new Product
                {
                    Artikelkenmerken = productDto.Variety ?? "",
                    Hoeveelheid = productDto.Quantity,
                    MinimumPrijs = productDto.MinPrice,
                    Dagdatum = DateTime.UtcNow,
                    LeverancierID = leverancierId,
                    Foto = fotoUrl
                };

                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                return Ok(product);
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