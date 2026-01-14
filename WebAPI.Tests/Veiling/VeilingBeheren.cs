using backend.DTO;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TreeMarket_Klas4_Group7.Controllers;
using security = System.Security.Claims;
using WebAPI.Tests.Helpers;


namespace WebAPI.Tests.TVeiling
{
    public class VeilingBeheren
    {
        // 1. Testen of je meerdere veilingne kunt zien.
        [Fact]
        public async Task GetAll_Returns_Multiple_Veilingen()
        {
            // Arrange
            var mockService = new Mock<IVeilingService>();
            mockService
              .Setup(s => s.GetAllAsync())
              .ReturnsAsync(new List<VeilingResponseDto>
              {
                new VeilingResponseDto { VeilingID = 1, ProductNaam = "Roos", StartPrijs = 10 },
                new VeilingResponseDto { VeilingID = 2, ProductNaam = "Paardebloem", StartPrijs = 12 }
              });

            var controller = new VeilingController(
                mockService.Object,
                null // object wordt al opgehaald binnen de UserManager
            );

            // Act
            var result = await controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<VeilingResponseDto>>(ok.Value);
            Assert.Equal(2, data.Count());
        }




        // 2. Test of je met een veilingsmeester kunt veilen
        [Fact]
        public async Task CreateVeiling_Succeeds_For_Veilingsmeester()
        {
            // Arrange
            var mockService = new Mock<IVeilingService>();
            mockService
             .Setup(s => s.GetAllAsync())
             .ReturnsAsync(new List<VeilingResponseDto>
             {
                new VeilingResponseDto
                {
                    VeilingID = 1,
                    ProductNaam = "Vlees-Eetende plant",
                    StartPrijs = 10
                }
             });


            var userManager = TestAuthHelper.MockUserManager("Veilingsmeester");

            var controller = new VeilingController(mockService.Object, userManager);

            controller.ControllerContext = TestAuthHelper.CreateContext("user-123", "Veilingsmeester");

            // Act
            var result = await controller.CreateVeiling(new VeilingDto());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }




        // ----------Autorisatie test-------------
        // 3. Test of je niet met een klant of leverancier kunt veilen.

        //Voor de klant
        [Fact]
        public async Task CreateVeiling_Returns_Forbid_For_Klant()
        {
            //Arrange
            var mockService = new Mock<IVeilingService>();
            var userManager = TestAuthHelper.MockUserManager("Klant");

            var controller = new VeilingController(mockService.Object, userManager);
            controller.ControllerContext = TestAuthHelper.CreateContext("user-123", "Klant");

            //Act
            var result = await controller.CreateVeiling(new VeilingDto());
            
            //Assert
            Assert.IsType<ForbidResult>(result);
        }

        //Voor de leverancier
        [Fact]
        public async Task CreateVeiling_Returns_Forbid_For_Leverancier()
        {
            //Arrange
            var mockService = new Mock<IVeilingService>();
            var userManager = TestAuthHelper.MockUserManager("Leverancier");

            var controller = new VeilingController(mockService.Object, userManager);
            controller.ControllerContext = TestAuthHelper.CreateContext("user-456", "Leverancier");

            //Act
            var result = await controller.CreateVeiling(new VeilingDto());

            //Arrange
            Assert.IsType<ForbidResult>(result);
        }
    }
}
