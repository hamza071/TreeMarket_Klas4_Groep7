using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Services
{
    public class DashboardService : IDashboardController
    {
        private readonly ApiContext _context;

        public DashboardService(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dashboard>> GetAllAsync()
        {
            return await _context.Dashboard.ToListAsync();
        }

        public async Task<Dashboard?> GetByIdAsync(int id)
        {
            return await _context.Dashboard.FindAsync(id);
        }

        public async Task<Dashboard> AddAsync(Dashboard dashboard)
        {
            _context.Dashboard.Add(dashboard);
            await _context.SaveChangesAsync();
            return dashboard;
        }

        public async Task<bool> UpdateAsync(int id, Dashboard dashboard)
        {
            if (id != dashboard.DashboardID) return false;

            _context.Entry(dashboard).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Dashboard.FindAsync(id);
            if (existing == null) return false;

            _context.Dashboard.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Dashboard.AnyAsync(x => x.DashboardID == id);
        }
    }
}
