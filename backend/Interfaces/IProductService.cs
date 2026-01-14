using backend.DTO;
using backend.Models;

namespace backend.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductMetVeilingmeesterDto>> GetVandaag();
        // Het alle producten van vandaag
        Task<List<ProductMetVeilingmeesterDto>> GetMetLeverancier();

        // Get alle producten met supllier
        Task<ProductMetVeilingmeesterDto> PostProduct(ProductUploadDto productDto, string userId, bool isAdmin);

        // Get de product met ID
        Task<ProductMetVeilingmeesterDto?> GetProductById(int id);

        // Delete alle producten van vandaag en return aantal verwijderde records
        Task<int> DeleteVandaag();

        // Delete single product by id, return true if deleted
        Task<bool> DeleteProduct(int id);
    }

}