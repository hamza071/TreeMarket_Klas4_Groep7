using backend.DTO;
using backend.Models;

namespace backend.Interfaces
{
    public interface IClaimService
    {
        // Voor de GET request
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<ProductHistoryResponse> GetHistoryAsync(string productNaam, string leverancierNaam);

        // Voor de POST request (AANGEPAST)
        // We veranderen de naam naar VerwerkAankoopAsync zodat hij matcht met je Service en Controller.
        // We veranderen return type naar bool (gelukt/niet gelukt).
        Task<bool> VerwerkAankoopAsync(ClaimDto dto, string userId);

        // Voor de DELETE request
        Task<bool> DeleteClaimAsync(int id);
    }
}