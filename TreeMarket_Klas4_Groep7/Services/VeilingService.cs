/* 
PSEUDOCODE / PLAN (detailed):
1. Ensure this service implements the interface method names expected by IVeilingController.
   - Rename/Create method `CreateVeilingAsync` (was `CreateVeilingOnlyAsync`) to match the interface.
2. Remove all code paths that return `null` where the method signature is non-nullable.
   - When an entity (Veiling) is not found, throw a specific exception (KeyNotFoundException).
   - This prevents CS8603 (possible null return) compiler warnings while keeping behavior explicit.
3. Validate incoming bid amounts:
   - If a bid is not greater than the current price, throw InvalidOperationException (caller can translate to 400).
4. Keep persistence behavior: add entities to DbContext, update `HuidigePrijs`, call `SaveChangesAsync`.
5. Keep other methods minimal and idiomatic async/EF Core usage.
6. Add required using directives for exceptions and collections.

Implementation details:
- `GetByIdAsync(int)`:
  - Find veiling.
  - If not found -> throw KeyNotFoundException.
  - Return veiling.
- `CreateVeilingAsync(VeilingDto, string)`:
  - Map dto fields to new Veiling.
  - Set initial HuidigePrijs = StartPrijs and Status = true.
  - Add to context and SaveChangesAsync.
  - Return created veiling.
- `PlaceBidAsync(CreateBidDTO, string)`:
  - Find veiling; if missing -> throw KeyNotFoundException.
  - If `dto.Bod` <= `veiling.HuidigePrijs` -> throw InvalidOperationException.
  - Create Bid with KlantId = userId, Tijdstip = UtcNow.
  - Add bid, update veiling.HuidigePrijs, SaveChangesAsync, return bid.
- `UpdateStatusAsync(int, bool)`:
  - Find veiling; if missing -> throw KeyNotFoundException.
  - Update Status, SaveChangesAsync, return veiling.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Interfaces;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace TreeMarket_Klas4_Groep7.Services
{
    public class VeilingService : IVeilingController
    {
        private readonly ApiContext _context;

        public VeilingService(ApiContext context)
        {
            _context = context;
        }

        public async Task<Veiling> CreateVeilingAsync(ProductMetLeverancierDto dto, string userId)
        {
            // 1️⃣ Haal eerst bestaand product op
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) throw new Exception("Product niet gevonden");

            // 2️⃣ Maak veiling aan
            var veiling = new Veiling
            {
                ProductID = product.ProductId,
                StartPrijs = dto.MinimumPrijs,
                HuidigePrijs = dto.MinimumPrijs,
                PrijsStap = 1, // of vul dit uit dto als je dat hebt
                VeilingsmeesterID = userId,
                Status = true
            };

            await _context.Veiling.AddAsync(veiling);
            await _context.SaveChangesAsync();

            return veiling;
        }

        public async Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId)
        {
            var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
            if (veiling == null) throw new KeyNotFoundException($"Veiling with id {dto.VeilingID} not found.");

            // Validate that the bid is higher than the current price
            if (dto.Bod <= veiling.HuidigePrijs)
                throw new InvalidOperationException("Bod moet hoger zijn dan de huidige prijs.");

            var bid = new Bid
            {
                VeilingID = dto.VeilingID,
                Bedrag = dto.Bod,
                Tijdstip = DateTime.UtcNow,
                KlantId = userId
            };

            _context.Bids.Add(bid);
            veiling.HuidigePrijs = dto.Bod;

            await _context.SaveChangesAsync();
            return bid;
        }

        public async Task<Veiling> UpdateStatusAsync(int veilingId, bool status)
        {
            var veiling = await _context.Veiling.FindAsync(veilingId);
            if (veiling == null) throw new KeyNotFoundException($"Veiling with id {veilingId} not found.");

            veiling.Status = status;
            await _context.SaveChangesAsync();

            return veiling;
        }
    }
}
