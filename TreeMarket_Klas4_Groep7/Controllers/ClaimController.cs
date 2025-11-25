using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

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

        // GET: api/Claims
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaim()
        {
            try
            {
                var result = await _context.Gebruiker.ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Claim kan niet opgehaald worden.", error = ex.Message });
            }
        }

        // GET: api/Claims/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> GetClaim(int id)
        {
            try
            {
                var claim = await _context.Claim.FindAsync(id);

                if (claim == null)
                {
                    return NotFound();
                }

                return claim;
            } 
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Claim op basis van een ID kan niet opgehaald worden.", error = ex.Message });
            }

        }

        // PUT: api/Claims/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClaim(int id, Claim claim)
        {
            if (id != claim.ClaimID)
            {
                return BadRequest();
            }

            _context.Entry(claim).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClaimExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Claims
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //ClaimDTO om gewoon Claim aan te maken
        [HttpPost("CreateClaim")]
        public async Task<IActionResult> PostClaim(ClaimDto claimDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // check of Klant en Veiling bestaan
            var klantBestaat = await _context.Gebruiker.FindAsync(claimDto.klantId);
            var veilingBestaat = await _context.Veiling.FindAsync(claimDto.veilingId);

            if (klantBestaat == null) return BadRequest("Klant bestaat niet.");
            if (veilingBestaat == null) return BadRequest("Veiling bestaat niet.");

            try
            {
                var claim = new Claim
                {
                    Prijs = claimDto.prijs,
                    KlantId = claimDto.klantId,
                    VeilingId = claimDto.veilingId
                };

                await _context.Claim.AddAsync(claim);
                await _context.SaveChangesAsync();

                return (Ok(claim));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Claim kan niet aangemaakt worden.", error = ex.Message });

            }
        }

        [HttpPost("ShowData")]
        public async Task<IActionResult> CreateClaim(Claim claim)
        {
            try
            {
                // Validatie: check dat alle required velden ingevuld zijn
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Voeg leverancier toe aan database
                await _context.Claim.AddAsync(claim);
                await _context.SaveChangesAsync();

                // Return het aangemaakte object (inclusief ID)
                return Ok(claim);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
            }
        }

        //ClaimToKlantDTO maken om de bestaande gebruikers te verbinden.

        // DELETE: api/Claims/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            _context.Claim.Remove(claim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClaimExists(int id)
        {
            return _context.Claim.Any(e => e.ClaimID == id);
        }
    }
}
