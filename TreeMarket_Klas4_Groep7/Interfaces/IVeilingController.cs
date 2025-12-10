using TreeMarket_Klas4_Groep7.Models.DTO;
using TreeMarket_Klas4_Groep7.Models;

public interface IVeilingController
{
    Task<Veiling> CreateVeilingAsync(ProductMetLeverancierDto dto, string userId);
    Task<List<Veiling>> GetAllAsync();
    Task<Veiling> GetByIdAsync(int veilingId);
    Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId);
    Task<Veiling> UpdateStatusAsync(int veilingId, bool status);
}