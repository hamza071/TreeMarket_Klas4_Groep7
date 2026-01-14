using backend.Data;
using backend.DTO;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductService : IProductService
    {
        private readonly ApiContext _context;

        public ProductService(ApiContext context)
        {
            _context = context;
        }

        
        [HttpGet("vandaag")]
        public async Task<List<ProductMetVeilingmeesterDto>> GetVandaag()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Product
                .Where(p => p.Dagdatum.Date == today)
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


        
        [HttpPost("CreateProduct")]
        [Authorize]
        public async Task<ProductMetVeilingmeesterDto> PostProduct(ProductUploadDto productDto, string userId, bool isAdmin)
        {
            // Foto uploaden
            string fotoUrl;
            if (productDto.Foto != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(productDto.Foto.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productDto.Foto.CopyToAsync(stream);
                }

                fotoUrl = "/images/" + uniqueFileName;
            }
            else
            {
                fotoUrl = "/images/default.png";
            }

            // Leverancier ophalen
            var leverancier = await _context.Leverancier.FirstOrDefaultAsync(l => l.Id == userId);
            string leverancierId = leverancier?.Id ?? (isAdmin ? "admin-01" : null);
            if (leverancierId == null)
                throw new Exception("Er bestaat geen Leverancier-profiel voor deze gebruiker.");

            // Product aanmaken
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

        
        [HttpGet("{id}")]
        public async Task<ProductMetVeilingmeesterDto?> GetProductById(int id)
        {
            var product = await _context.Product
                .Include(p => p.Leverancier)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return null; 

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
                LeverancierNaam = product.Leverancier?.Bedrijf
            };
        }

      
        [HttpDelete("vandaag")]
        public async Task<int> DeleteVandaag()
        {
            var today = DateTime.UtcNow.Date;

            var toDelete = await _context.Product
                .Where(p => p.Dagdatum.Date == today)
                .ToListAsync();

            if (!toDelete.Any()) return 0;

            _context.Product.RemoveRange(toDelete);
            var removed = await _context.SaveChangesAsync();

            return removed;
        }

    
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null) return false;

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}