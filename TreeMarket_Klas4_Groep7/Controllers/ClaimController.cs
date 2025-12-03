using Microsoft.AspNetCore.Authorization; // <--- BELANGRIJK
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Claim = TreeMarket_Klas4_Groep7.Models.Claim;


namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly ApiContext _context;

        public ClaimController(ApiContext context)
        {
            _context = context;
        }

        // GET: api/Claim
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaims()
        {
            try
            {
                // Include Klant en Veiling voor meer info
                return await _context.Claim
                    .Include(c => c.klant)
                    .Include(c => c.Veiling)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // POST: api/Claim/CreateClaim
        [HttpPost("CreateClaim")]
        [Authorize] // <--- ALLEEN INGELOGDE GEBRUIKERS!
        public async Task<IActionResult> PostClaim([FromBody] ClaimDto claimDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Haal de ingelogde gebruiker ID op uit het token (Identity)
            // Dit is VEEL veiliger dan de ID uit de DTO te halen.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Geeft de GUID string

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Je moet ingelogd zijn om te bieden.");
            }

            try
            {
                var claim = new Claim
                {
                    Prijs = claimDto.prijs,
                    VeilingId = claimDto.veilingId,
                    
                    // 2. Vul de ID van de ingelogde gebruiker in
                    KlantId = userId // Dit is nu een string, en dat klopt!
                };

                await _context.Claim.AddAsync(claim);
                await _context.SaveChangesAsync();

                return Ok(claim);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Claim mislukt.", error = ex.Message });
            }
        }

        // DELETE: api/Claim/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Alleen admin mag claims verwijderen?
        public async Task<IActionResult> DeleteClaim(int id)
        {
            try 
            {
                var claim = await _context.Claim.FindAsync(id);
                if (claim == null) return NotFound();

                _context.Claim.Remove(claim);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Fout bij verwijderen.", error = ex.Message });
            }
        }
    }
}