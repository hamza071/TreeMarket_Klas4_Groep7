using backend.Data;
using backend.Models;
using backend.Models.DTO;
using Microsoft.EntityFrameworkCore;
using backend.Interfaces;

namespace backend.Services
{
    public class ProductService : IProductController
    {
        private readonly ApiContext _context;

        public ProductService(ApiContext context)
        {
            _context = context;
        }

        // ===================== Haal producten van vandaag =====================
        public async Task<List<ProductDto>> GetProductenVanVandaagAsync()
        {
            var vandaag = DateTime.Today;

            return await _context.Product
                .Where(p => p.Dagdatum.Date == vandaag)
                .OrderBy(p => p.MinimumPrijs)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Foto = p.Foto,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimumPrijs = p.MinimumPrijs
                })
                .ToListAsync();
        }

        // ===================== Haal producten met leverancier info =====================
        public async Task<List<ProductMetLeverancierDto>> GetProductenMetLeverancierAsync()
        {
            return await _context.Product
                .Include(p => p.Leverancier)
                .Select(p => new ProductMetLeverancierDto
                {
                    ProductId = p.ProductId,
                    MinimumPrijs = p.MinimumPrijs,
                    LeverancierNaam = p.Leverancier != null ? p.Leverancier.Bedrijf : null
                })
                .ToListAsync();
        }

        // ===================== Voeg product toe of update bestaand product =====================
        public async Task<Product?> AddOrUpdateProductAsync(Product product)
        {
            // ✅ Validatie
            if (string.IsNullOrWhiteSpace(product.ProductNaam)) return null;
            if (product.Hoeveelheid <= 0) return null;
            if (product.MinimumPrijs < 0) return null;
            if (product.Dagdatum.Date < DateTime.Today) return null;
            if (string.IsNullOrEmpty(product.LeverancierID)) return null;

            if (product.ProductId == 0)
            {
                await _context.Product.AddAsync(product);
            }
            else
            {
                var productInDb = await _context.Product.FindAsync(product.ProductId);
                if (productInDb != null)
                {
                    productInDb.Foto = product.Foto;
                    productInDb.ProductNaam = product.ProductNaam;
                    productInDb.Varieteit = product.Varieteit;
                    productInDb.Omschrijving = product.Omschrijving;
                    productInDb.Hoeveelheid = product.Hoeveelheid;
                    productInDb.MinimumPrijs = product.MinimumPrijs;
                    productInDb.Dagdatum = product.Dagdatum;
                    productInDb.LeverancierID = product.LeverancierID;
                }
                else
                {
                    return null; // Product niet gevonden
                }
            }

            await _context.SaveChangesAsync();
            return product;
        }

        // ===================== Haal product op basis van ID =====================
        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _context.Product.FindAsync(productId);
        }

        // ===================== Verwijder product =====================
        public async Task<bool> DeleteAsync(int productId)
        {
            var product = await _context.Product.FindAsync(productId);
            if (product == null) return false;

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}