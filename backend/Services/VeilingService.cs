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

        // Haal alle veilingen op (Eist de Interface)
        public async Task<List<Veiling>> GetAllAsync()
        {
            return await _context.Veiling
                .Include(v => v.Product)
                .ToListAsync();
        }

        // Haal 1 veiling op (Eist de Interface)
        public async Task<Veiling> GetByIdAsync(int id)
        {
            var veiling = await _context.Veiling
                .Include(v => v.Product)
                // HIER IS DE FIX: VeilingID in plaats van Id
                .FirstOrDefaultAsync(v => v.VeilingID == id); 

            if (veiling == null)
            {
                throw new KeyNotFoundException($"Veiling met ID {id} niet gevonden.");
            }

            return veiling;
        }

        // Maak veiling aan (Aangepast naar VeilingDto om error in Controller te fixen)
        public async Task<Veiling> CreateVeilingAsync(VeilingDto dto, string userId)
        {
            // AANGEPAST: dto.ProductID (met hoofdletter D, zoals in je DTO)
            var product = await _context.Products.FindAsync(dto.ProductID); 
    
            if (product == null) throw new KeyNotFoundException("Product niet gevonden");

            var veiling = new Veiling
            {
                ProductID = product.ProductId,
                StartPrijs = dto.StartPrijs,
                HuidigePrijs = dto.StartPrijs,
                // Tip: Je DTO heeft ook 'PrijsStap', dus die kun je hier gebruiken in plaats van hardcoded '1'
                PrijsStap = dto.PrijsStap, 
                VeilingsmeesterID = userId,
                Status = true
            };

            await _context.Veiling.AddAsync(veiling);
            await _context.SaveChangesAsync();

            return veiling;
        }
        public async Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId)
        {
            // OOK HIER DE FIX: FindAsync zoekt automatisch op de Primary Key (VeilingID)
            var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
            
            if (veiling == null) throw new KeyNotFoundException($"Veiling with id {dto.VeilingID} not found.");

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