//using Microsoft.AspNetCore.Authorization; // <--- NODIG VOOR BEVEILIGING
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims; // <--- NODIG OM ID UIT TOKEN TE HALEN
//using TreeMarket_Klas4_Groep7.Data;
//using TreeMarket_Klas4_Groep7.Models;
//using TreeMarket_Klas4_Groep7.Models.DTO;

//namespace TreeMarket_Klas4_Group7.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class VeilingController : ControllerBase
//    {
//        private readonly ApiContext _context;

//        public VeilingController(ApiContext context)
//        {
//            _context = context;
//        }

//        // ================= GET (Openbaar) =================

//        // GET: api/veiling
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            try
//            {
//                var veilingen = await _context.Veiling.ToListAsync();
//                return Ok(veilingen);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
//            }
//        }

//        // GET: api/veiling/5
//        [HttpGet("{id}")]
//        public async Task<IActionResult> Get(int id)
//        {
//            try
//            {
//                var veiling = await _context.Veiling.FindAsync(id);

//                if (veiling == null)
//                    return NotFound(new { message = $"Veiling met ID {id} niet gevonden." });

//                return Ok(veiling);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Databasefout: Kon veiling niet ophalen.", error = ex.Message });
//            }
//        }

//        // ================= CREATE (Alleen Veilingsmeester/Admin) =================

//        [HttpPost("CreateVeiling")]
//        [Authorize(Roles = "Veilingsmeester, Admin")] // <--- BEVEILIGING
//        public async Task<IActionResult> Create(VeilingToDo dto)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);

//            // 1. Haal de ID van de ingelogde gebruiker op uit het token
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (userId == null) return Unauthorized("Je bent niet ingelogd.");

//            try
//            {
//                var veiling = new Veiling
//                {
//                    StartPrijs = dto.StartPrijs,
//                    HuidigePrijs = dto.StartPrijs,
//                    PrijsStap = dto.PrijsStap,
//                    // PrijsStrategie = dto.PrijsStrategie, // Voeg toe als je die in DTO hebt
//                    ProductID = dto.ProductID,
//                    StartTijd = dto.StartTijd,
//                    EindTijd = dto.EindTijd,

//                    // AANGEPAST: We gebruiken de ID uit het token, niet uit de DTO!
//                    VeilingsmeesterID = userId,

//                    Status = true,
//                    // TimerInSeconden = dto.TimerInSeconden // Voeg toe als je die in DTO hebt
//                };

//                await _context.Veiling.AddAsync(veiling);
//                await _context.SaveChangesAsync();

//                return Ok(veiling);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Databasefout: Veiling kon niet worden aangemaakt.", error = ex.Message });
//            }
//        }

//        // ================= BIEDEN (Alleen Ingelogde Klanten) =================

//        // POST: api/veiling/Bid
//        [HttpPost("Bid")]
//        [Authorize] // <--- Iedereen met een account mag bieden
//        public async Task<IActionResult> PlaceBid(CreateBidDTO dto)
//        {
//            // 1. Haal ID van de klant op
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (userId == null) return Unauthorized("Je moet ingelogd zijn om te bieden.");

//            try
//            {
//                var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
//                if (veiling == null) return NotFound("Veiling niet gevonden.");

//                // (Optioneel: Check hier of het bod wel hoger is dan HuidigePrijs)
//                // if (dto.Bod <= veiling.HuidigePrijs) return BadRequest("Bod moet hoger zijn.");

//                var bid = new Bid
//                {
//                    VeilingID = dto.VeilingID,
//                    Bedrag = dto.Bod,
//                    Tijdstip = DateTime.UtcNow,

//                    // AANGEPAST: Koppel het bod aan de ingelogde klant (Identity String ID)
//                    KlantId = userId
//                };

//                _context.Bids.Add(bid);

//                // Update de veiling prijs
//                veiling.HuidigePrijs = dto.Bod;

//                await _context.SaveChangesAsync();

//                return Ok(bid);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Databasefout: Kon bod niet plaatsen.", error = ex.Message });
//            }
//        }

//        // ================= UPDATE (Alleen Veilingsmeester/Admin) =================

//        // PUT: api/veiling/UpdateStatus/5
//        [HttpPut("UpdateStatus/{id}")]
//        [Authorize(Roles = "Veilingsmeester, Admin")]
//        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDTO dto)
//        {
//            try
//            {
//                var veiling = await _context.Veiling.FindAsync(id);
//                if (veiling == null) return NotFound("Veiling niet gevonden.");

//                veiling.Status = dto.Status;
//                await _context.SaveChangesAsync();

//                return Ok(veiling);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Databasefout: Kon status niet aanpassen.", error = ex.Message });
//            }
//        }
//    }
//}