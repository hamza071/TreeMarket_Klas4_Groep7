using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

        // GET: api/veiling
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Veiling.ToListAsync());
        }

        // GET: api/veiling/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var veiling = await _context.Veiling.FindAsync(id);
            if (veiling == null) return NotFound();
            return Ok(veiling);
        }

        [HttpPost("CreateVeiling")]
        public async Task<IActionResult> Create(VeilingToDo dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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

        // POST: api/veiling/Bid
        [HttpPost("Bid")]
        public async Task<IActionResult> PlaceBid(CreateBidDTO dto)
        {
            var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
            if (veiling == null) return NotFound("Veiling niet gevonden.");

            var bid = new Bid
            {
                VeilingID = dto.VeilingID,
                Bedrag = dto.Bod
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            veiling.HuidigePrijs = dto.Bod;
            await _context.SaveChangesAsync();

            return Ok(bid);
        }

        // PUT: api/veiling/UpdateStatus/5
        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDTO dto)
        {
            var veiling = await _context.Veiling.FindAsync(id);
            if (veiling == null) return NotFound("Veiling niet gevonden.");

            veiling.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(veiling);
        }
    }
}