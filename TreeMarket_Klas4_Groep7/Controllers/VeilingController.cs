using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// using SQLitePCL; 
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
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

        //=====POST======
        //ClaimDTO om gewoon Claim aan te maken
        [HttpPost("CreateVeiling")]
        public async Task<IActionResult> PostVeiling(VeilingToDo veilingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var veiling = new Veiling
                {
                    StartPrijs = veilingDto.StartPrijs,
                    PrijsStap = veilingDto.PrijsStap,
                    StartTijd = veilingDto.StartTijd,
                    EindTijd = veilingDto.EindTijd,
                    ProductID = veilingDto.ProductID, 
                    VeilingsmeesterID = veilingDto.VeilingsmeesterID 
                };

                await _context.Veiling.AddAsync(veiling);
                await _context.SaveChangesAsync();

                return (Ok(veiling));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veiling kan niet aangemaakt worden.", error = ex.Message });

            }

        }

        [HttpPost("ShowData")]
        public async Task<IActionResult> CreateVeiling(Veiling veiling)
        {
            try
            {
                // Validatie: check dat alle required velden ingevuld zijn
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Voeg leverancier toe aan database
                await _context.Veiling.AddAsync(veiling);
                await _context.SaveChangesAsync();

                // Return het aangemaakte object (inclusief ID)
                return Ok(veiling);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
            }
        }

    }
}