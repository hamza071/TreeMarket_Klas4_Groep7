using backend.Models;
using backend.Models.DTO;

namespace backend.Interfaces
{
    public interface IClaimService
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim> CreateClaimAsync(ClaimDto dto, string userId);
        Task<bool> DeleteClaimAsync(int id);
    }
}
