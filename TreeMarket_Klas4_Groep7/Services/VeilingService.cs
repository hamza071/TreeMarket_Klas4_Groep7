using Humanizer;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Services
{
    public class VeilingService : IVeilingController
    {
        private readonly ApiContext _context;

        public VeilingService(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Veiling>> GetAllAsync()
        {
            return await _context.Veiling.ToListAsync();
        }

        public async Task<Veiling> GetByIdAsync(int id)
        {
            return await _context.Veiling.FindAsync(id);
        }

        public async Task<Veiling> AddAsync(Veiling veiling)
        {
            await _context.Veiling.AddAsync(veiling);
            await _context.SaveChangesAsync();
            return veiling;
        }

        public async Task<Veiling> UpdateAsync(Veiling veiling)
        {
            _context.Veiling.Update(veiling);
            await _context.SaveChangesAsync();
            return veiling;
        }

        public async Task<Bid> AddBidAsync(Bid bid)
        {
            await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
            return bid;
        }
    }
}
