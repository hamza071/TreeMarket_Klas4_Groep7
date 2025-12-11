using backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Services
{
    public class ClaimService : IClaimController
    {
        private readonly ApiContext _context;

        public ClaimService(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await _context.Claim
                .Include(c => c.klant)
                .Include(c => c.Veiling)
                .ToListAsync();
        }

        public async Task<Claim> CreateClaimAsync(ClaimDto dto, string userId)
        {
            var claim = new Claim
            {
                Prijs = dto.prijs,
                VeilingId = dto.veilingId,
                KlantId = userId   // van de token!
            };

            await _context.Claim.AddAsync(claim);
            await _context.SaveChangesAsync();

            return claim;
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim == null) return false;

            _context.Claim.Remove(claim);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
