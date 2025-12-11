using backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Services
{
    public class LeverancierService : ILeverancierController
    {
        private readonly UserManager<Gebruiker> _userManager;

        public LeverancierService(UserManager<Gebruiker> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<Leverancier>> GetAllAsync()
        {
            return await _userManager.Users
                .OfType<Leverancier>()
                .ToListAsync();
        }

        public async Task<Leverancier> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id) as Leverancier;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id) as Leverancier;
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }
    }
}
