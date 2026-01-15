using backend.DTO;

namespace backend.Interfaces
{
    public interface IProductService
    {
        // GET
        Task<List<ProductMetVeilingmeesterDto>> GetVandaag();
        Task<List<ProductMetVeilingmeesterDto>> GetMetLeverancier();
        Task<ProductMetVeilingmeesterDto?> GetProductById(int id);

        // POST
        Task<ProductMetVeilingmeesterDto> CreateProduct(
            ProductUploadDto productDto,
            string userId,
            bool isAdmin
        );

        // Backwards-compatible alias used by tests/earlier code
        Task<ProductMetVeilingmeesterDto> PostProduct(
            ProductUploadDto productDto,
            string userId,
            bool isAdmin
        );

        // DELETE
        Task<int> DeleteTodayProducts();
        Task<bool> DeleteProduct(int id);
    }
}