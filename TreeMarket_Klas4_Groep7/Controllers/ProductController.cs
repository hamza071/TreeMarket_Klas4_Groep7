using backend.Interfaces;
using Microsoft.AspNetCore.Authorization; // <--- NODIG VOOR BEVEILIGING
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

        public ProductController(IProductController service)
        {
            _service = service;
        }

        // ================= GET ENDPOINTS (OPENBAAR) =================

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

        // ================= CREATE / UPDATE (BEVEILIGD) =================

        [HttpPost]
        //[Authorize]
        //[Authorize(Roles = "Leverancier, Admin")]
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] Product product)
        {
            try
            {
                var result = await _service.AddOrUpdateProductAsync(product);
                if (result == null)
                    return BadRequest("Validatie mislukt of product niet gevonden.");
                return Ok(result);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // validatiefouten
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // update van niet-bestaand product
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }


        // DIT IS DE BETERE METHODE (gebruikt DTO en Token)
        [HttpPost("CreateProduct")]
        //[Authorize]
        //[Authorize(Roles = "Leverancier, Admin")]
        public async Task<IActionResult> PostProduct([FromForm] ProductUploadDto productDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("Je bent niet ingelogd.");

            try
            {
                string? fotoUrl = null;

                // Opslaan van de afbeelding als bestand
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

                    fotoUrl = "/images/" + uniqueFileName; // URL naar frontend
                }

                var product = new Product
                {
                    Artikelkenmerken = productDto.Variety,
                    Hoeveelheid = productDto.Quantity,
                    MinimumPrijs = productDto.MinPrice,
                    Dagdatum = DateTime.UtcNow,
                    LeverancierID = userId,
                    Foto = fotoUrl // ✅ hier een string
                };

                var result = await _service.AddOrUpdateProductAsync(product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }
    }
}