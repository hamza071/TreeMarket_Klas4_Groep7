using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Controllers.Interfaces;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Services
{
    //IT service desk voor de gebruikersController. 
    //Als de gebruikersController iets nodig heeft, kan hij het hier ophalen.
    public class GebruikerService : IGebruikerController
    {
        private readonly ApiContext _context;

        public GebruikerService(ApiContext context)
        {
            _context = context;
        }

        public async Task<Gebruiker> GetByEmailAsync(string email)
        {
            return await _context.Gebruiker.FirstOrDefaultAsync(g => g.Email.ToLower() == email.ToLower());
        }

        public async Task AddAsync(Gebruiker gebruiker)
        {
            await _context.Gebruiker.AddAsync(gebruiker);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Gebruiker>> GetAllAsync()
        {
            return await _context.Gebruiker.ToListAsync();
        }

        public async Task<Gebruiker> GetByIdAsync(int id)
        {
            return await _context.Gebruiker.FindAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var result = await _context.Gebruiker.FindAsync(id);
            if (result != null) 
            {
                _context.Gebruiker.Remove(result);
                await _context.SaveChangesAsync();
            }
        }
    }
}
