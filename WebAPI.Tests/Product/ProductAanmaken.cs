using backend.Controllers;
using backend.DTO;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using security = System.Security.Claims;
using WebAPI.Tests.Helpers;

namespace WebAPI.Tests.TProduct
{
    public class ProductAanmaken
    {
        [Fact]
        public async Task ProductKanAangemaaktWorden_Succes()
        {
            // Arrange
            var mockService = new Mock<IProductService>();

            var expectedDto = new ProductMetVeilingmeesterDto
            {
                ProductId = 1,
                Naam = "Test product",
                Hoeveelheid = 5,
                MinimumPrijs = 10,
                Foto = "image.jpg",
                Status = "pending",
                LeverancierNaam = "Test Leverancier"
            };

            mockService
              .Setup(s => s.CreateProduct(
                  It.IsAny<ProductUploadDto>(),
                  "leverancier-123",
                  false))
              .ReturnsAsync(expectedDto);


            var controller = new ProductController(mockService.Object);

            // Mock user (Leverancier)
            controller.ControllerContext = TestAuthHelper.CreateContext("leverancier-123", "Leverancier");

            var dto = new ProductUploadDto
            {
                ProductNaam = "Test product",
                Varieteit = "Appel",
                Hoeveelheid = 5,
                MinimumPrijs = 10,
                Foto = null
            };

            // Act
            var result = await controller.CreateProduct(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // anonymous object uitpakken
            var value = okResult.Value!;
            var productProperty = value.GetType().GetProperty("product");
            Assert.NotNull(productProperty);

            var product = Assert.IsType<ProductMetVeilingmeesterDto>(
                productProperty!.GetValue(value)
            );

            Assert.Equal(1, product.ProductId);
            Assert.Equal("image.jpg", product.Foto);
            Assert.Equal(5, product.Hoeveelheid);
            Assert.Equal(10, product.MinimumPrijs);
            Assert.Equal("pending", product.Status);
        }

    }
}