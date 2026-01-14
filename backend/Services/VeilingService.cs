using backend.Data;
using backend.DTO;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class VeilingService : IVeilingService
    {
        private readonly ApiContext _context;

        public VeilingService(ApiContext context)
        {
            _context = context;
        }

        // Haal alle actieve veilingen op (DTO voor frontend)
        public async Task<List<VeilingResponseDto>> GetAllAsync()
        {
            // 1. Haal alle actieve veilingen inclusief Product EN LEVERANCIER
            var veilingen = await _context.Veiling
                .Include(v => v.Product)
                    .ThenInclude(p => p.Leverancier) 
                .Where(v => v.Status)
                .ToListAsync();

            // 2. Map in memory naar DTO
            return veilingen.Select(v => new VeilingResponseDto
            {
                VeilingID = v.VeilingID,
                Status = v.Status,
                StartPrijs = v.StartPrijs,
                HuidigePrijs = v.HuidigePrijs,
                MinPrijs = v.MinPrijs,
                TimerInSeconden = v.TimerInSeconden,
                ProductID = v.ProductID,
                ProductNaam = v.Product?.ProductNaam ?? "",
                Foto = v.Product?.Foto ?? "",
                StartTimestamp = v.StartTimestamp,
                Hoeveelheid = v.Product?.Hoeveelheid ?? 0,
                Omschrijving = v.Product?.Omschrijving ?? "",

                // === HIER DE AANPASSING ===
                // We mappen de bedrijfsnaam van de leverancier naar de DTO.
                // Als er geen leverancier is gekoppeld, sturen we "Onbekend".
                LeverancierNaam = v.Product?.Leverancier?.Bedrijf ?? "Onbekend"
            }).ToList();
        }

        // Haal 1 veiling op
        public async Task<Veiling> GetByIdAsync(int id)
        {
            var veiling = await _context.Veiling
                .Include(v => v.Product)
                    .ThenInclude(p => p.Leverancier) // Ook hier handig om te hebben
                .FirstOrDefaultAsync(v => v.VeilingID == id);

            if (veiling == null)
                throw new KeyNotFoundException($"Veiling met ID {id} niet gevonden.");

            return veiling;
        }

        // Maak veiling aan
        public async Task<VeilingResponseDto> CreateVeilingAsync(VeilingDto dto, string userId)
        {
            // 1. Product ophalen MET leverancier
            var product = await _context.Product
                .Include(p => p.Leverancier)
                .FirstOrDefaultAsync(p => p.ProductId == dto.ProductID);

            if (product == null)
                throw new KeyNotFoundException("Product niet gevonden.");

            // 2. StartTimestamp bepalen: geplande of directe start
            // Normalize incoming StartTimestamp to UTC to avoid Kind mismatches
            var requestedStart = dto.StartTimestamp.Kind == DateTimeKind.Utc
                ? dto.StartTimestamp
                : DateTime.SpecifyKind(dto.StartTimestamp, DateTimeKind.Utc);

            var startTimestamp = requestedStart > DateTime.UtcNow
                ? requestedStart // geplande veiling
                : DateTime.UtcNow;   // directe start

            // 3. Veiling aanmaken
            var veiling = new Veiling
            {
                StartPrijs = dto.StartPrijs,
                HuidigePrijs = dto.StartPrijs,
                MinPrijs = dto.MinPrijs,
                TimerInSeconden = dto.TimerInSeconden,
                StartTimestamp = startTimestamp,
                //EindTimestamp = startTimestamp.AddSeconds(dto.TimerInSeconden), // handig voor featured/completed logic  
                Status = true,
                ProductID = product.ProductId,
                VeilingsmeesterID = userId
            };

            _context.Veiling.Add(veiling);
            await _context.SaveChangesAsync();

            // 4. Response DTO
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
                StartTimestamp = veiling.StartTimestamp,
                //EindTimestamp = veiling.EindTimestamp,
                Hoeveelheid = product.Hoeveelheid,

                // Direct goed teruggeven bij aanmaken
                LeverancierNaam = product.Leverancier?.Bedrijf ?? "Onbekend"
            };
        }

        // Plaats een bod
        public async Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId)
        {
            var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
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
            var veiling = await _context.Veiling.FindAsync(veilingId);
            if (veiling == null)
                throw new KeyNotFoundException($"Veiling met ID {veilingId} niet gevonden.");

            veiling.Status = status;
            await _context.SaveChangesAsync();

            return veiling;
        }

        // Verwijder veiling
        public async Task DeleteVeilingAsync(int veilingId)
        {
            var veiling = await _context.Veiling.FindAsync(veilingId);
            if (veiling == null)
                throw new KeyNotFoundException($"Veiling met ID {veilingId} niet gevonden.");

            _context.Veiling.Remove(veiling);
            await _context.SaveChangesAsync();
        }
    }
}