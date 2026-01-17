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
using WebAPI.Tests.Helpers;
using security = System.Security.Claims;


namespace WebAPI.Tests.TProduct
{
    public class ProductValidatievelden
    {
        // LeverancierID mag niet NULL zijn
        [Fact]
        public async Task ProductKanNietAangemaaktWorden_ZonderToken()
        {
            //Assert
            var mockService = new Mock<IProductService>();
            var controller = new ProductController(mockService.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                         new ClaimsIdentity() 
                    )
                }
            };

            var dto = new ProductUploadDto
            {
                ProductNaam = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 10
            };

            //Act
            var result = await controller.CreateProduct(dto);

            //Assert
            Assert.IsType<UnauthorizedResult>(result);
        }


        //Klant mag geen product aanmaken
        [Fact]
        public async Task KlantKanProductNietAanmaken()
        {
            //Arrange
            var mockService = new Mock<IProductService>();
            var controller = new ProductController(mockService.Object);

            controller.ControllerContext = TestAuthHelper.CreateContext("klant-123", "Klant");

            var dto = new ProductUploadDto
            {
                ProductNaam = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 10
            };

            //Act
            var result = await controller.CreateProduct(dto);

            //Assert
            Assert.IsType<ForbidResult>(result);
        }


        //Veilingsmeester en Admin mogen ook geen producten aanmaken 
        [Fact]
        public async Task VeilingsmeesterKanProductNietAanmaken()
        {
            //Assert
            var mockService = new Mock<IProductService>();
            var controller = new ProductController(mockService.Object);

            controller.ControllerContext = TestAuthHelper.CreateContext("veilingsmeester-123", "Veilingsmeester");


            var dto = new ProductUploadDto
            {
                ProductNaam = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 10
            };

            //Act
            var result = await controller.CreateProduct(dto);

            //Arrange
            Assert.IsType<ForbidResult>(result);
        }

    }
}