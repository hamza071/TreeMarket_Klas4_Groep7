using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

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

        // ================= GET ENDPOINTS =================

        // GET: api/veiling
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var veilingen = await _context.Veiling
                .Include(v => v.Product)       // optioneel: include product info
                .Include(v => v.Veilingsmeester) // optioneel: include veilingmeester info
                .ToListAsync();

            return Ok(veilingen);
        }

        // GET: api/veiling/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var veiling = await _context.Veiling
                .Include(v => v.Product)
                .Include(v => v.Veilingsmeester)
                .FirstOrDefaultAsync(v => v.VeilingID == id);

            if (veiling == null) return NotFound(new { message = "Veiling niet gevonden." });

            return Ok(veiling);
        }

        // ================= CREATE =================

        // POST: api/veiling/CreateVeiling
        [HttpPost("CreateVeiling")]
        public async Task<IActionResult> CreateVeiling([FromBody] VeilingToDo dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Controleer of product bestaat
            var product = await _context.Product.FindAsync(dto.ProductID);
            if (product == null) return NotFound(new { message = "Product niet gevonden." });

            // Optional: check of veilingsmeester bestaat (bijv. Identity user)
            var veiling = new Veiling
            {
                StartPrijs = dto.StartPrijs,
                HuidigePrijs = dto.StartPrijs,
                PrijsStap = dto.PrijsStap,
                ProductID = dto.ProductID,
                VeilingsmeesterID = dto.VeilingsmeesterID,
                Status = true,
                TimerInSeconden = dto.TimerInSeconden
            };

            _context.Veiling.Add(veiling);
            await _context.SaveChangesAsync();

            return Ok(veiling);
        }

        // ================= BID =================

        // POST: api/veiling/Bid
        [HttpPost("Bid")]
        public async Task<IActionResult> PlaceBid([FromBody] CreateBidDTO dto)
        {
            var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
            if (veiling == null) return NotFound(new { message = "Veiling niet gevonden." });

            // Optioneel: controleer dat bod hoger is dan huidige prijs
            if (dto.Bod <= veiling.HuidigePrijs)
                return BadRequest(new { message = "Bod moet hoger zijn dan de huidige prijs." });

            var bid = new Bid
            {
                VeilingID = dto.VeilingID,
                Bedrag = dto.Bod
            };

            _context.Bids.Add(bid);

            // Update veiling
            veiling.HuidigePrijs = dto.Bod;

            await _context.SaveChangesAsync();

            return Ok(bid);
        }

        // ================= UPDATE STATUS =================

        // PUT: api/veiling/UpdateStatus/5
        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO dto)
        {
            var veiling = await _context.Veiling.FindAsync(id);
            if (veiling == null) return NotFound(new { message = "Veiling niet gevonden." });

            veiling.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(veiling);
        }
    }
}