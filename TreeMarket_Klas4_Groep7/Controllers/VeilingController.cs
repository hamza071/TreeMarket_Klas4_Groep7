using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// using SQLitePCL; 
using TreeMarket_Klas4_Groep7.Data;

namespace TreeMarket_Klas4_Group7.Controllers
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
        
        // Haalt alle veilingen op
        [HttpGet]
        public async Task<IActionResult> GetAllVeilingen()
        {
            try
            {
                var veilingen = await _context.Veiling.ToListAsync();

                return Ok(veilingen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVeilingById(int id)
        {
            try
            {
                var veiling = await _context.Veiling.FindAsync(id);

                if (veiling == null)
                {
                    return NotFound(new { message = $"Veiling met ID {id} niet gevonden." });
                }

                return Ok(veiling);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
            }
        }
        
        
    }
}//