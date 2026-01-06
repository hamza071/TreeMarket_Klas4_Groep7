using backend.DTO;
using backend.Models;

namespace backend.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductMetVeilingmeesterDto>> GetVandaag();
        // Get all products for today
        Task<List<ProductMetVeilingmeesterDto>> GetMetLeverancier();

        // Get all products including supplier info
        Task<ProductMetVeilingmeesterDto> PostProduct(ProductUploadDto productDto, string userId, bool isAdmin);

        // Get a product by its ID
        Task<ProductMetVeilingmeesterDto?> GetProductById(int id);
    }

}