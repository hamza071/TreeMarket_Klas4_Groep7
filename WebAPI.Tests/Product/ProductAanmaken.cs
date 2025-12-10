using backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using security =  System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TreeMarket_Klas4_Groep7.Controllers;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace WebAPI.Tests.TProduct
{
    public class ProductAanmaken
    {
        [Fact]
        public async Task ProductKanAangemaaktWorden_Succes()
        {
            // Arrange
            var mockService = new Mock<IProductController>();

            mockService.Setup(s => s.AddOrUpdateProductAsync(It.IsAny<Product>()))
                       .Returns<Product>(p =>
                       {
                           p.ProductId = 1;
                           return Task.FromResult(p);
                       });

            var controller = new ProductController(mockService.Object);

            // --- MOCK DE GEBRUIKER (BELANGRIJK!) ---
            var user = new security.ClaimsPrincipal(new security.ClaimsIdentity(new security.Claim[]
            {
                new security.Claim(security.ClaimTypes.NameIdentifier, "leverancier-123")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var productDto = new ProductDto
            {
                Foto = "image.jpg",
                artikelkenmerken = "Test kenmerken",
                Hoeveelheid = 5,
                MinimumPrijs = 10,
                dagdatum = DateTime.Now,
                leverancierID = "leverancier-123" // leverancier is nodig om een product aan te kunnen maken.
            };

            // Act
            var result = await controller.PostProduct(productDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(1, product.ProductId);
            Assert.Equal("image.jpg", product.Foto);
            Assert.Equal("Test kenmerken", product.Artikelkenmerken);
            Assert.Equal(5, product.Hoeveelheid);
            Assert.Equal(10, product.MinimumPrijs);

            // Token user ID moet gebruikt worden:
            Assert.Equal("leverancier-123", product.LeverancierID);
        }

    }
}