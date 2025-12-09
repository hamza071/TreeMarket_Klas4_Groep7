using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using TreeMarket_Klas4_Groep7.Services;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimController _service;

        public ClaimController(IClaimController service)
        {
            _service = service;
        }

        // GET: api/Claims
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaim()
        {
            try
            {
                var result = await _service.GetAllAsync();
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
                var claim = await _service.GetByIdAsync(id);

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
            var updated = await _service.UpdateAsync(id, claim);

            if (!updated)
                return NotFound();

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

            //// check of Klant en Veiling bestaan
            //var klantBestaat = await _context.Gebruiker.FindAsync(claimDto.klantId);
            //var veilingBestaat = await _context.Veiling.FindAsync(claimDto.veilingId);

            //if (klantBestaat == null) return BadRequest("Klant bestaat niet.");
            //if (veilingBestaat == null) return BadRequest("Veiling bestaat niet.");

            try
            {
                await _service.CreateAsync(claimDto);
                return (Ok(claimDto));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Claim kan niet aangemaakt worden.", error = ex.Message });

            }
        }

        //ClaimToKlantDTO maken om de bestaande gebruikers te verbinden.

        // DELETE: api/Claims/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound();

            return NoContent();
        }

        private async Task<bool> ClaimExists(int id)
        {
            //return _context.Claim.Any(e => e.ClaimID == id);
            return await _service.ExistsAsync(id);
        }
    }
}
