using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Services
{
    public class ClaimService: IClaimController
    {
        private readonly ApiContext _context;

        public ClaimService(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _context.Claim.ToListAsync();
        }

        public async Task<Claim?> GetByIdAsync(int id)
        {
            return await _context.Claim.FindAsync(id);
        }

        public async Task<Claim> CreateAsync(ClaimDto dto)
        {
            var claim = new Claim
            {
                Prijs = dto.prijs,
                KlantId = dto.klantId,
                VeilingId = dto.veilingId
            };

            await _context.Claim.AddAsync(claim);
            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task<bool> UpdateAsync(int id, Claim claim)
        {
            if (id != claim.ClaimID)
                return false;

            if (!await ExistsAsync(id))
                return false;

            _context.Entry(claim).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim == null) return false;

            _context.Claim.Remove(claim);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return _context.Claim.Any(c => c.ClaimID == id);
        }
    }
}
