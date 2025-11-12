using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using System.Linq;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeverancierController : ControllerBase
    {
        private readonly ApiContext _context;

        // ✅ Constructor: krijgt de databasecontext via Dependency Injection
        public LeverancierController(ApiContext context)
        {
            _context = context;
        }

        // ===============================
        // POST: Voeg een nieuwe leverancier toe
        // Endpoint: POST /api/Leverancier
        // ===============================
        [HttpPost]
        public IActionResult CreateLeverancier(Leverancier leverancier)
        {
            // Validatie: check dat alle required velden ingevuld zijn
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Voeg leverancier toe aan database
            _context.Leverancier.Add(leverancier);
            _context.SaveChanges();

            // Return het aangemaakte object (inclusief ID)
            return Ok(leverancier);
        }

        // ===============================
        // GET: Haal alle leveranciers op
        // Endpoint: GET /api/Leverancier
        // ===============================
        [HttpGet]
        public IActionResult GetAllLeveranciers()
        {
            var leveranciers = _context.Leverancier.ToList();
            return Ok(leveranciers);
        }

        // ===============================
        // GET: Haal leverancier op basis van ID
        // Endpoint: GET /api/Leverancier/{id}
        // ===============================
        [HttpGet("{id}")]
        public IActionResult GetLeverancierById(int id)
        {
            var leverancier = _context.Leverancier.Find(id);

            if (leverancier == null)
                return NotFound("Leverancier niet gevonden met ID: " + id);

            return Ok(leverancier);
        }
    }
}
