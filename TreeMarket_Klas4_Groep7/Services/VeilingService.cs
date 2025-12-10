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

        public async Task<List<Veiling>> GetAllAsync()
        {
            return await _context.Veiling.ToListAsync();
        }

        public async Task<Veiling> GetByIdAsync(int veilingId)
        {
            return await _context.Veiling.FindAsync(veilingId);
        }

        public async Task<Veiling> CreateVeilingAsync(CreateVeilingWithProductDto dto, string userId)
        {
            // 1️⃣ Maak eerst het product aan
            var product = new Product
            {
                ProductNaam = dto.Naam, // als je entity 'ProductNaam' heet
                Variëteit = dto.Variëteit,
                KleurOfSoort = dto.KleurOfSoort,
                AantalStuks = dto.AantalStuks,
                Omschrijving = dto.Omschrijving,
                MinimumPrijs = dto.MinimumPrijs,
                AfbeeldingUrl = dto.AfbeeldingUrl,
                LeverancierId = dto.LeverancierId
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync(); // zodat product.Id beschikbaar is

            // 2️⃣ Maak daarna de veiling aan die verwijst naar dit product
            var veiling = new Veiling
            {
                ProductID = product.ProductId,   // koppel product
                StartPrijs = dto.StartPrijs,
                HuidigePrijs = dto.StartPrijs,
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
            var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
            if (veiling == null) return null;

            // (Optioneel: Check hier of het bod wel hoger is dan HuidigePrijs)
            // if (dto.Bod <= veiling.HuidigePrijs) return BadRequest("Bod moet hoger zijn.");
            var bid = new Bid
            {
                VeilingID = dto.VeilingID,
                Bedrag = dto.Bod,
                Tijdstip = DateTime.UtcNow,

                // AANGEPAST: Koppel het bod aan de ingelogde klant (Identity String ID)
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
            if (veiling == null) return null;

            veiling.Status = status;
            await _context.SaveChangesAsync();

            return veiling;
        }
    }
}
