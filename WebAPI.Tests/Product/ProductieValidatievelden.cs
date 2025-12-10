//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TreeMarket_Klas4_Groep7.Controllers;
//using TreeMarket_Klas4_Groep7.Interfaces;
//using TreeMarket_Klas4_Groep7.Models;
//using TreeMarket_Klas4_Groep7.Models.DTO;

//namespace WebAPI.Tests.TProduct
//{
//    public class ProductValidatievelden
//    {
//        // ============LeverancierID mag niet NULL zijn =============
//        [Fact]
//        public async Task ProductKanNietAangemaaktWorden_ZonderLeverancier()
//        {
//            // Arrange
//            var mockService = new Mock<IProductController>();
//            mockService.Setup(s => s.AddAsync(It.IsAny<Product>()))
//                       .Returns<Product>(p => Task.FromResult(p));

//            var controller = new ProductController(mockService.Object);

//            var productDto = new ProductDto
//            {
//                Foto = "image.jpg",
//                artikelkenmerken = "Test kenmerken",
//                Hoeveelheid = 5,
//                MinimumPrijs = 10,
//                dagdatum = DateTime.Now,
//                leverancierID = 0 // fout: leverancier ontbreekt
//            };

//            // Act
//            var result = await controller.PostProduct(productDto);

//            // Assert
//            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
//            Assert.Equal("LeverancierID is verplicht.", badRequest.Value);
//        }

//    }
//}