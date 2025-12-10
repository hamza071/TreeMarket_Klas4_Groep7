using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace backend.Interfaces
{
    public interface IVeilingController
    {
        Task<List<Veiling>> GetAllAsync();
        Task<Veiling> GetByIdAsync(int veilingId);
        Task<Veiling> CreateVeilingAsync(VeilingDto dto, string userId);
        Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId);
        Task<Veiling> UpdateStatusAsync(int veilingId, bool status);
    }
}
