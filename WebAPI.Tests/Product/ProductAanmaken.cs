//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TreeMarket_Klas4_Groep7.Controllers;
//using backend.Interfaces;
//using TreeMarket_Klas4_Groep7.Models;
//using TreeMarket_Klas4_Groep7.Models.DTO;

//namespace WebAPI.Tests.TProduct
//{
//    public class ProductAanmaken
//    {
//        [Fact]
//        public async Task ProductKanAangemaaktWorden_Succes()
//        {
//            // Arrange
//            var mockService = new Mock<IProductController>();

//            // Mock AddAsync zodat het product een ID krijgt (simuleer database auto-increment)
//            mockService.Setup(s => s.AddAsync(It.IsAny<Product>()))
//                       .Returns<Product>(p =>
//                       {
//                           p.ProductId = 1;
//                           return Task.FromResult(p);
//                       });

//            var controller = new ProductController(mockService.Object);

//            var productDto = new ProductDto
//            {
//                Foto = "image.jpg",
//                artikelkenmerken = "Test kenmerken",
//                Hoeveelheid = 5,
//                MinimumPrijs = 10,
//                dagdatum = DateTime.Now,
//                leverancierID = 2000
//            };

//            // Act
//            var result = await controller.PostProduct(productDto);

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var product = Assert.IsType<Product>(okResult.Value);

//            Assert.Equal(1, product.ProductId); // ID door mock gesimuleerd
//            Assert.Equal("image.jpg", product.Foto);
//            Assert.Equal("Test kenmerken", product.Artikelkenmerken);
//            Assert.Equal(5, product.Hoeveelheid);
//            Assert.Equal(10, product.MinimumPrijs);
//            Assert.Equal(2000, product.LeverancierID);
//        }
//    }
//}