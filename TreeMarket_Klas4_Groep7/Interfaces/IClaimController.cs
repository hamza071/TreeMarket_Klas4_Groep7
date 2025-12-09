using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Interfaces
{
    public interface IClaimController
    {
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim> GetByIdAsync(int id);
        Task<Claim> CreateAsync(ClaimDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(int id, Claim claim);
        Task<bool> ExistsAsync(int id);
    }
}
