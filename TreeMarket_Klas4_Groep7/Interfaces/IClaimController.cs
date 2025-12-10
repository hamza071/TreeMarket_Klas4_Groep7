using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace backend.Interfaces
{
    public interface IClaimController
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim> CreateClaimAsync(ClaimDto dto, string userId);
        Task<bool> DeleteClaimAsync(int id);
    }
}
