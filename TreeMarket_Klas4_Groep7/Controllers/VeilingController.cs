using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Group7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeilingController : ControllerBase
    {
        private readonly IVeilingController _service;

        public VeilingController(IVeilingController service)
        {
            _service = service;
        }

        // GET: api/veiling
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        // GET: api/veiling/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var veiling = await _service.GetByIdAsync(id);
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

            await _service.AddAsync(veiling);

            return Ok(veiling);
        }

        // POST: api/veiling/Bid
        [HttpPost("Bid")]
        public async Task<IActionResult> PlaceBid(CreateBidDTO dto)
        {
            var veiling = await _service.GetByIdAsync(dto.VeilingID);
            if (veiling == null) return NotFound("Veiling niet gevonden.");

            var bid = new Bid
            {
                VeilingID = dto.VeilingID,
                Bedrag = dto.Bod
            };

            //In de service opslaan
            await _service.AddBidAsync(bid);

            veiling.HuidigePrijs = dto.Bod;
            await _service.AddAsync(veiling);

            return Ok(bid);
        }

        // PUT: api/veiling/UpdateStatus/5
        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDTO dto)
        {
            var veiling = await _service.GetByIdAsync(id);
            if (veiling == null) return NotFound("Veiling niet gevonden.");

            veiling.Status = dto.Status;
            await _service.UpdateAsync(veiling);

            return Ok(veiling);
        }
    }
}