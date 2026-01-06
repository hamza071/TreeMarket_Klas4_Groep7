using backend.Interfaces;
using Microsoft.AspNetCore.Authorization; // <--- NODIG VOOR BEVEILIGING
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using TreeMarket_Klas4_Groep7.Services;

namespace backend.Controllers
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

        // ================= CREATE / UPDATE =================
        [HttpPost("CreateProduct")]
        [Authorize] // alleen ingelogde gebruikers mogen
        public async Task<IActionResult> PostProduct([FromForm] ProductUploadDto productDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("Je bent niet ingelogd.");

        [HttpPost]
        [Authorize(Roles = "Leverancier, Admin")]
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] Product product)
        {
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
                    
                    // AANGEPAST: Gebruik de ID uit het token!
                    LeverancierID = userId 
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
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _context.Product
                    .Include(p => p.Leverancier)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                    return NotFound(new { message = $"Product {id} niet gevonden." });

                var productDto = new ProductMetVeilingmeesterDto
                {
                    ProductId = product.ProductId,
                    Naam = product.ProductNaam,
                    Varieteit = product.Varieteit,
                    Omschrijving = product.Omschrijving,
                    Hoeveelheid = product.Hoeveelheid,
                    MinimumPrijs = product.MinimumPrijs,
                    Foto = product.Foto,
                    Status = "pending",
                    LeverancierNaam = product.Leverancier?.Bedrijf
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Serverfout.", error = ex.Message });
            }
        }

    }
}