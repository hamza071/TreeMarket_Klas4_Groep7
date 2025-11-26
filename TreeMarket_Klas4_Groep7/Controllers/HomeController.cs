using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly ApiContext _context;

        public AuctionsController(ApiContext context)
        {
            _context = context;
        }

        //================ GET =================
        // Haal alle veilingitems op
        [HttpGet]
        public async Task<IActionResult> GetAllAuctions()
        {
            try
            {
                var auctions = await _context.Auctions.ToListAsync();
                return Ok(auctions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veilingen kunnen niet opgehaald worden.", error = ex.Message });
            }
        }

        // Haal één veilingitem op basis van ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionById(int id)
        {
            try
            {
                var auction = await _context.Auctions.FindAsync(id);
                if (auction == null)
                    return NotFound("Veiling niet gevonden: " + id);
                return Ok(auction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veiling kan niet opgehaald worden.", error = ex.Message });
            }
        }

        //================ POST =================
        // Nieuwe veiling aanmaken
        [HttpPost]
        public async Task<IActionResult> CreateAuction([FromBody] AuctionDto auctionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var auction = new Auction
                {
                    Titel = auctionDto.Titel,
                    Beschrijving = auctionDto.Beschrijving,
                    StartPrijs = auctionDto.StartPrijs,
                    EindDatum = auctionDto.EindDatum
                };

                await _context.Auctions.AddAsync(auction);
                await _context.SaveChangesAsync();

                return Ok(auction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veiling kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        //================ PUT =================
        // Update bestaande veiling
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuction(int id, [FromBody] AuctionDto auctionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
                return NotFound("Veiling niet gevonden: " + id);

            try
            {
                auction.Titel = auctionDto.Titel;
                auction.Beschrijving = auctionDto.Beschrijving;
                auction.StartPrijs = auctionDto.StartPrijs;
                auction.EindDatum = auctionDto.EindDatum;

                _context.Auctions.Update(auction);
                await _context.SaveChangesAsync();

                return Ok(auction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veiling kan niet geüpdatet worden.", error = ex.Message });
            }
        }

        //================ DELETE =================
        // Verwijder veiling
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
                return NotFound("Veiling niet gevonden: " + id);

            try
            {
                _context.Auctions.Remove(auction);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veiling kan niet verwijderd worden.", error = ex.Message });
            }
        }
    }
}