using backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models; // Zorg dat deze erbij staat voor 'Gebruiker'

namespace TreeMarket_Klas4_Groep7.Services
{
    public class GebruikerService: IGebruikerController
    {
        private readonly ApiContext _context;
        private readonly UserManager<Gebruiker> _userManager;

        public GebruikerService(ApiContext context, UserManager<Gebruiker> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Gebruiker> GetByEmailAsync(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(g => g.Email.ToLower() == email.ToLower());
        }

        public async Task AddUserAsync(Gebruiker gebruiker, string wachtwoord, string role)
        {
            // 1. Maak gebruiker aan (Wachtwoord wordt hier automatisch gehasht!)
            var result = await _userManager.CreateAsync(gebruiker, wachtwoord);
            // 2. Rol toewijzen
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(gebruiker, role);
        }

        public async Task<IEnumerable<Gebruiker>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<Gebruiker> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }


        // Deze methode controleert of de mail bestaat of niet.
        public async Task<bool> EmailBestaatAl(string email)
        {
            // AANGEPAST: .Gebruiker -> .Users
            // IdentityDbContext noemt de tabel met gebruikers altijd 'Users'
            return await _userManager.Users.AnyAsync(g => g.Email.ToLower() == email.ToLower());
        }
    }
}