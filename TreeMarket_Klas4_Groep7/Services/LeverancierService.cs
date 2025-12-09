using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Services
{
    public class LeverancierService : ILeverancierController
    {
        private readonly ApiContext _context;

        public LeverancierService(ApiContext context)
        {
            _context = context;
        }

        public async Task<Leverancier> AddAsync(Leverancier leverancier)
        {
            await _context.Leverancier.AddAsync(leverancier);
            await _context.SaveChangesAsync();
            return leverancier;
        }

        public async Task<IEnumerable<Leverancier>> GetAllAsync()
        {
            return await _context.Leverancier.ToListAsync();
        }

        public async Task<Leverancier?> GetByIdAsync(int id)
        {
            return await _context.Leverancier.FindAsync(id);
        }
    }
}
