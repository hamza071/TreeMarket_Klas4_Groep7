using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;
using backend.Models.DTO;

namespace backend.Interfaces
{
    public interface IVeilingService
    {
        Task<VeilingResponseDto> CreateVeilingAsync(VeilingDto dto, string userId);

        // Hier aangepast van List<Veiling> naar List<VeilingResponseDto>
        Task<List<VeilingResponseDto>> GetAllAsync();

        Task<Veiling> GetByIdAsync(int veilingId);
        Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId);
        Task<Veiling> UpdateStatusAsync(int veilingId, bool status);
        Task DeleteVeilingAsync(int veilingId);
    }
}