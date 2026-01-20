 using backend.Interfaces;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;
 using Moq;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Text;
 using System.Threading.Tasks;
 using backend.Controllers;
 using backend.Models;
 using backend.Services;
 using security = System.Security.Claims;
 using backend.DTO;
using WebAPI.Tests.Helpers;


namespace WebAPI.Tests.TClaim
 {
     public class ClaimAanmakenMetToken
     {
       //Claim kan alleen aangemaakt worden wanneer de gebruiker een token heeft 
        [Fact]
        public async Task PlaceClaim_Gebruikt_UserId_Uit_Token()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();

            ClaimDto receivedDto = null;
            string receivedUserId = null;

            mockService
                .Setup(s => s.VerwerkAankoopAsync(It.IsAny<ClaimDto>(), It.IsAny<string>()))
                .Callback<ClaimDto, string>((dto, userId) =>
                {
                    receivedDto = dto;
                    receivedUserId = userId;
                })
                .ReturnsAsync(true);

            var controller = new ClaimController(mockService.Object);
            controller.ControllerContext = TestAuthHelper.CreateContext("user-123", "");

            var claimDto = new ClaimDto
            {
                VeilingId = 5,
                Prijs = 100,
                Aantal = 2
            };

            // Act
            var result = await controller.PlaceClaim(claimDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(receivedDto);
            Assert.Equal(5, receivedDto.VeilingId);
            Assert.Equal(100, receivedDto.Prijs);
            Assert.Equal(2, receivedDto.Aantal);

            Assert.Equal("user-123", receivedUserId);
        }


        //Het zou bij niet-ingelogde gebruiker moeten falen 
        [Fact]
        public async Task PlaceClaim_Faalt_ZonderToken()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();
            var controller = new ClaimController(mockService.Object);

            controller.ControllerContext = TestAuthHelper.CreateContext("", "");

            var claimDto = new ClaimDto
            {
                VeilingId = 5,
                Prijs = 100,
                Aantal = 1
            };

            // Act
            var result = await controller.PlaceClaim(claimDto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Je moet ingelogd zijn om te bieden.",
                unauthorized.Value.GetType().GetProperty("message")?.GetValue(unauthorized.Value));
        }

    }
}