using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeilingController : ControllerBase
    {
        private readonly ApiContext _context;

        public VeilingController(ApiContext context)
        {
            _context = context;
        }

        // ✅ Haal alle veilingen op
        [HttpGet]
        public async Task<IActionResult> GetAllVeilingen()
        {
            try
            {
                var veilingen = await _context.Veiling
                    .Include(v => v.Product)
                    .Include(v => v.Veilingsmeester)
                    .ToListAsync();

                return Ok(veilingen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
            }
        }

        // ✅ Haal veiling op via ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVeilingById(int id)
        {
            try
            {
                var veiling = await _context.Veiling
                    .Include(v => v.Product)
                    .Include(v => v.Veilingsmeester)
                    .FirstOrDefaultAsync(v => v.VeilingID == id);

                if (veiling == null)
                    return NotFound(new { message = $"Veiling met ID {id} niet gevonden." });

                return Ok(veiling);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veiling niet ophalen.", error = ex.Message });
            }
        }

        // ✅ Maak nieuwe veiling aan (pending)
        [HttpPost]
        public async Task<IActionResult> CreateVeiling([FromBody] Veiling nieuweVeiling)
        {
            if (nieuweVeiling == null)
                return BadRequest(new { message = "Veilinggegevens ontbreken." });

            // Controleer of het product bestaat
            var productExists = await _context.Product.AnyAsync(p => p.ProductId == nieuweVeiling.ProductID);
            if (!productExists)
                return BadRequest(new { message = "ProductID bestaat niet. Maak eerst het product aan." });

            // Minimum validatie
            if (nieuweVeiling.Lots <= 0)
                return BadRequest(new { message = "Aantal lots moet groter zijn dan 0." });

            if (string.IsNullOrWhiteSpace(nieuweVeiling.Status))
                nieuweVeiling.Status = "pending"; // standaard status

            try
            {
                await _context.Veiling.AddAsync(nieuweVeiling);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetVeilingById), new { id = nieuweVeiling.VeilingID }, nieuweVeiling);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veiling niet aanmaken.", error = ex.Message });
            }
        }
    }
}