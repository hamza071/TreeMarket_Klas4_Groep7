//using backend.Interfaces;
//using TreeMarket_Klas4_Groep7.Data;
//using TreeMarket_Klas4_Groep7.Models;
//using TreeMarket_Klas4_Groep7.Models.DTO;
//using Microsoft.EntityFrameworkCore;

//namespace TreeMarket_Klas4_Groep7.Services
//{
//    public class VeilingService : IVeilingController
//    {
//        private readonly ApiContext _context;

//        public VeilingService(ApiContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<Veiling>> GetAllAsync()
//        {
//            return await _context.Veiling.ToListAsync();
//        }

//        public async Task<Veiling> GetByIdAsync(int veilingId)
//        {
//            return await _context.Veiling.FindAsync(veilingId);
//        }

//        public async Task<Veiling> CreateVeilingAsync(VeilingDto dto, string userId)
//        {
//            var veiling = new Veiling
//            {
//                StartPrijs = dto.StartPrijs,
//                HuidigePrijs = dto.StartPrijs,
//                PrijsStap = dto.PrijsStap,
//                ProductID = dto.ProductID,
//                StartTijd = DateTime.UtcNow,
//                EindTijd = DateTime.UtcNow.AddSeconds(dto.TimerInSeconden),
//                VeilingsmeesterID = userId, // altijd token-ID gebruiken
//                Status = true
//            };

//            await _context.Veiling.AddAsync(veiling);
//            await _context.SaveChangesAsync();

//            return veiling;
//        }

//        public async Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId)
//        {
//            var veiling = await _context.Veiling.FindAsync(dto.VeilingID);
//            if (veiling == null) return null;

//            // (Optioneel: Check hier of het bod wel hoger is dan HuidigePrijs)
//            // if (dto.Bod <= veiling.HuidigePrijs) return BadRequest("Bod moet hoger zijn.");
//            var bid = new Bid
//            {
//                VeilingID = dto.VeilingID,
//                Bedrag = dto.Bod,
//                Tijdstip = DateTime.UtcNow,

//                // AANGEPAST: Koppel het bod aan de ingelogde klant (Identity String ID)
//                KlantId = userId
//            };

//            _context.Bids.Add(bid);
//            veiling.HuidigePrijs = dto.Bod;

//            await _context.SaveChangesAsync();
//            return bid;
//        }

//        public async Task<Veiling> UpdateStatusAsync(int veilingId, bool status)
//        {
//            var veiling = await _context.Veiling.FindAsync(veilingId);
//            if (veiling == null) return null;

//            veiling.Status = status;
//            await _context.SaveChangesAsync();

//            return veiling;
//        }
//    }
//}
