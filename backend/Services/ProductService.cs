using backend.Data;
using backend.DTO;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ProductService : IProductService
    {
        private readonly ApiContext _context;

        public ProductService(ApiContext context)
        {
            _context = context;
        }

        // GET: api/Product/vandaag
        [HttpGet("vandaag")]
        public async Task<List<ProductMetVeilingmeesterDto>> GetVandaag()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Product
                .Where(p => p.Dagdatum.Date == today)
                .Include(p => p.Leverancier)
                .Select(p => new ProductMetVeilingmeesterDto
                {
                    ProductId = p.ProductId,
                    Naam = p.ProductNaam,
                    Varieteit = p.Varieteit,
                    Omschrijving = p.Omschrijving,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimumPrijs = p.MinimumPrijs,
                    Foto = p.Foto,
                    Status = "pending",
                    LeverancierNaam = p.Leverancier != null ? p.Leverancier.Bedrijf : null
                })
                .ToListAsync();
        }



        // GET: api/Product/leverancier
        [HttpGet("leverancier")]
        public async Task<List<ProductMetVeilingmeesterDto>> GetMetLeverancier()
        {
            return await _context.Product
                .Include(p => p.Leverancier)
                .Select(p => new ProductMetVeilingmeesterDto
                {
                    ProductId = p.ProductId,
                    Naam = p.ProductNaam,
                    Varieteit = p.Varieteit,
                    Omschrijving = p.Omschrijving,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimumPrijs = p.MinimumPrijs,
                    Foto = p.Foto,
                    Status = "pending",
                    LeverancierNaam = p.Leverancier != null ? p.Leverancier.Bedrijf : null
                })
                .ToListAsync();
        }

        public async Task<ProductMetVeilingmeesterDto?> GetProductById(int id)
        {
            var product = await _context.Product
                .Include(p => p.Leverancier)
                .FirstOrDefaultAsync(p => p.ProductId == id);

        // POST: api/Product/CreateProduct
        [HttpPost("CreateProduct")]
        [Authorize]
        public async Task<ProductMetVeilingmeesterDto> PostProduct(ProductUploadDto productDto, string userId, bool isAdmin)
        {
            // Foto uploaden
            string fotoUrl;
            if (productDto.Foto != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(productDto.Foto.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await productDto.Foto.CopyToAsync(stream);

                fotoUrl = "/images/" + fileName;
            }

            var leverancier = await _context.Leverancier
                .FirstOrDefaultAsync(l => l.Id == userId);

            var leverancierId = leverancier?.Id ?? (isAdmin ? "admin-01" : null);
            if (leverancierId == null)
                throw new Exception("Geen leverancier gevonden.");

            var product = new Product
            {
                ProductNaam = productDto.ProductNaam ?? "",
                Varieteit = productDto.Varieteit ?? "",
                Omschrijving = productDto.Omschrijving ?? "",
                Hoeveelheid = productDto.Hoeveelheid,
                MinimumPrijs = productDto.MinimumPrijs,
                Dagdatum = DateTime.UtcNow,
                LeverancierID = leverancierId,
                Foto = fotoUrl
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            // Return DTO
            return new ProductMetVeilingmeesterDto
            {
                ProductId = product.ProductId,
                Naam = product.ProductNaam,
                Varieteit = product.Varieteit,
                Omschrijving = product.Omschrijving,
                Hoeveelheid = product.Hoeveelheid,
                MinimumPrijs = product.MinimumPrijs,
                Foto = product.Foto,
                Status = "pending",
                LeverancierNaam = leverancier?.Bedrijf ?? (isAdmin ? "Admin" : null)
            };
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<ProductMetVeilingmeesterDto?> GetProductById(int id)
        {
            var product = await _context.Product
                .Include(p => p.Leverancier)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return null; // Controller can handle NotFound

            return new ProductMetVeilingmeesterDto
            {
                ProductId = product.ProductId,
                Naam = product.ProductNaam,
                Varieteit = product.Varieteit,
                Omschrijving = product.Omschrijving,
                Hoeveelheid = product.Hoeveelheid,
                MinimumPrijs = product.MinimumPrijs,
                Foto = product.Foto,
                Status = "pending",
                LeverancierNaam = leverancier?.Bedrijf ?? "Admin"
            };
        }

        // DELETE: api/Product/vandaag
        [HttpDelete("vandaag")]
        public async Task<int> DeleteVandaag()
        {
            var today = DateTime.UtcNow.Date;

            var products = await _context.Product
                .Where(p => p.Dagdatum.Date == today)
                .ToListAsync();

            if (!products.Any())
                return 0;

            _context.Product.RemoveRange(products);
            return await _context.SaveChangesAsync();
        }

        // Delete single product by id
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
                return false;

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}