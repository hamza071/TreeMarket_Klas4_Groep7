using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTO;
using backend.Models;

namespace backend.Interfaces
{
    public interface IVeilingService
    {
        Task<VeilingResponseDto> CreateVeilingAsync(VeilingDto dto, string userId);

       
        Task<List<VeilingResponseDto>> GetAllAsync();

        Task<Veiling> GetByIdAsync(int veilingId);
        Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId);
        Task<Veiling> UpdateStatusAsync(int veilingId, bool status);
        Task DeleteVeilingAsync(int veilingId);
    }
}