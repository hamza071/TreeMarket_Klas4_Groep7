using backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTO;

namespace backend.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApiContext _context;

        public ClaimService(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await _context.Claim // Let op: heet je tabel 'Claim' of 'Claims'? Pas aan indien nodig.
                .Include(c => c.klant)
                .Include(c => c.Veiling)
                    .ThenInclude(v => v.Product) // Handig om ook het product te zien
                .ToListAsync();
        }

        // DEZE IS NIEUW EN VERVANGT CreateClaimAsync
        public async Task<bool> VerwerkAankoopAsync(ClaimDto dto, string userId)
        {
            // 1. Haal de veiling op, INCLUSIEF het Product (want daar staat de voorraad in)
            var veiling = await _context.Veiling
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VeilingID == dto.VeilingId);

            // Checks: Bestaat de veiling en het product?
            if (veiling == null) throw new Exception("Veiling niet gevonden.");
            if (veiling.Product == null) throw new Exception("Product gegevens ontbreken bij deze veiling.");

            // Check: Is de veiling nog actief?
            if (veiling.Status != true) throw new Exception("Deze veiling is al gesloten.");

            // Check: Is het aantal geldig?
            if (dto.Aantal < 1) throw new Exception("Aantal moet minimaal 1 zijn.");

            // Check: Is er genoeg voorraad?
            if (veiling.Product.Hoeveelheid < dto.Aantal)
            {
                throw new Exception($"Niet genoeg voorraad. Er zijn er nog maar {veiling.Product.Hoeveelheid} beschikbaar.");
            }

            // 2. Alles is goed! Maak de Claim aan (De bestelling).
            var nieuweClaim = new Claim
            {
                KlantId = userId,
                VeilingId = dto.VeilingId,
                Prijs = dto.Prijs,
                Aantal = dto.Aantal
            };

            _context.Claim.Add(nieuweClaim);

            // 3. Update de voorraad van het product
            veiling.Product.Hoeveelheid = veiling.Product.Hoeveelheid - dto.Aantal;

            // 4. Als de voorraad nu op is, sluit de veiling direct
            if (veiling.Product.Hoeveelheid <= 0)
            {
                veiling.Product.Hoeveelheid = 0; // Voorkom negatieve getallen
                veiling.Status = false;          // Veiling sluiten
            }

            // 5. Sla alles (Claim + Product wijziging + Veiling wijziging) in één keer op
            await _context.SaveChangesAsync();

            return true;
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