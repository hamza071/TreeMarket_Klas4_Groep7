using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeverancierController : ControllerBase
    {
        private readonly ILeverancierController _service;

        //  Constructor: krijgt de databasecontext via Dependency Injection
        public LeverancierController(ILeverancierController service)
        {
            _service = service;
        }

        // ===============================
        // POST: Voeg een nieuwe leverancier toe
        // Bij deze methode wordt letterlijk alles verwacht ingevuld te worden. Niet hetzelfde als bij de GebruikersController.
        // Endpoint: POST /api/Leverancier
        // ===============================
        [HttpPost]
        public async Task<IActionResult> CreateLeverancier(Leverancier leverancier)
        {
            try
            {
                // Validatie: check dat alle required velden ingevuld zijn
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Voeg leverancier toe aan database
                await _service.AddAsync(leverancier);

                // Return het aangemaakte object (inclusief ID)
                return Ok(leverancier);
            } 
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
            }
            
        }

        // ===============================
        // GET: Haal alle leveranciers op
        // Endpoint: GET /api/Leverancier
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAllLeveranciers()
        {
            try
            {
                var leveranciers = await _service.GetAllAsync();
                return Ok(leveranciers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon leverancier niet ophalen.", error = ex.Message });
            }
            
        }

        // ===============================
        // GET: Haal leverancier op basis van ID
        // Endpoint: GET /api/Leverancier/{id}
        // ===============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeverancierById(int id)
        {
            try
            {
                var leverancier = await _service.GetByIdAsync(id);

                if (leverancier == null)
                    return NotFound("Leverancier niet gevonden met ID: " + id);

                return Ok(leverancier);
            } 
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Leverancier Id kon niet opgehaald worden.", error = ex.Message });
            }

        }
    }
}
