using backend.Models;
using backend.Models.DTO;

namespace backend.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductMetVeilingmeesterDto>> GetProductenVanVandaagAsync();
        Task<List<ProductMetVeilingmeesterDto>> GetProductenMetLeverancierAsync();

        Task<Product> CreateProductAsync(ProductUploadDto dto, string userId, bool isAdmin);
        Task<Product?> AddOrUpdateProductAsync(Product product);

        Task<ProductMetVeilingmeesterDto?> GetByIdAsync(int id);
    }

}
