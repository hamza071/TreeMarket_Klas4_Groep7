using System;
using System.Collections.Generic;
using System.Linq;
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

        // Haal alle actieve veilingen op (DTO voor frontend)
        public async Task<List<VeilingResponseDto>> GetAllAsync()
        {
            var veilingen = await _context.Veilingen
                .Include(v => v.Product)
                .Where(v => v.Status) // alleen actieve veilingen
                .ToListAsync();

            return veilingen.Select(v => new VeilingResponseDto
            {
                VeilingID = v.VeilingID,
                Status = v.Status,
                StartPrijs = v.StartPrijs,
                HuidigePrijs = v.HuidigePrijs,
                MinPrijs = v.MinPrijs,
                TimerInSeconden = v.TimerInSeconden,
                ProductID = v.ProductID,
                ProductNaam = v.Product.ProductNaam,
                Foto = v.Product.Foto,
                StartTimestamp = v.StartTimestamp
            }).ToList();
        }

        // Haal 1 veiling op
        public async Task<Veiling> GetByIdAsync(int id)
        {
            var veiling = await _context.Veilingen
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VeilingID == id);

            if (veiling == null)
                throw new KeyNotFoundException($"Veiling met ID {id} niet gevonden.");

            return veiling;
        }

        // Maak veiling aan
        public async Task<VeilingResponseDto> CreateVeilingAsync(VeilingDto dto, string userId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == dto.ProductID);

            if (product == null)
                throw new KeyNotFoundException("Product niet gevonden.");

            var veiling = new Veiling
            {
                StartPrijs = dto.StartPrijs,
                HuidigePrijs = dto.StartPrijs,
                MinPrijs = dto.MinPrijs, // ✅ belangrijk voor prijsafloop
                TimerInSeconden = dto.TimerInSeconden,
                StartTimestamp = DateTime.UtcNow,
                Status = true,
                ProductID = product.ProductId,
                VeilingsmeesterID = userId
            };

            _context.Veilingen.Add(veiling);
            await _context.SaveChangesAsync();

            return new VeilingResponseDto
            {
                VeilingID = veiling.VeilingID,
                Status = veiling.Status,
                StartPrijs = veiling.StartPrijs,
                HuidigePrijs = veiling.HuidigePrijs,
                MinPrijs = veiling.MinPrijs,
                TimerInSeconden = veiling.TimerInSeconden,
                ProductID = product.ProductId,
                ProductNaam = product.ProductNaam,
                Foto = product.Foto,
                StartTimestamp = veiling.StartTimestamp
            };
        }

        // Plaats een bod
        public async Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId)
        {
            var veiling = await _context.Veilingen.FindAsync(dto.VeilingID);
            if (veiling == null)
                throw new KeyNotFoundException($"Veiling met ID {dto.VeilingID} niet gevonden.");

            if (!veiling.Status)
                throw new InvalidOperationException("Veiling is gesloten.");

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

        // Update status veiling (open/gesloten)
        public async Task<Veiling> UpdateStatusAsync(int veilingId, bool status)
        {
            var veiling = await _context.Veilingen.FindAsync(veilingId);
            if (veiling == null)
                throw new KeyNotFoundException($"Veiling met ID {veilingId} niet gevonden.");

            veiling.Status = status;
            await _context.SaveChangesAsync();

            return veiling;
        }

        // Verwijder veiling
        public async Task DeleteVeilingAsync(int veilingId)
        {
            var veiling = await _context.Veilingen.FindAsync(veilingId);
            if (veiling == null)
                throw new KeyNotFoundException($"Veiling met ID {veilingId} niet gevonden.");

            _context.Veilingen.Remove(veiling);
            await _context.SaveChangesAsync();
        }
    }
}