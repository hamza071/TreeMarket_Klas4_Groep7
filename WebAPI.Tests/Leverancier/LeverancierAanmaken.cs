//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TreeMarket_Klas4_Groep7.Controllers;
//using backend.Interfaces;
//using TreeMarket_Klas4_Groep7.Services;
//using TreeMarket_Klas4_Groep7.Models;
//using TreeMarket_Klas4_Groep7.Models.DTO;

//namespace WebAPI.Tests.TLeverancier
//{
//    public class LeverancierAanmaken
//    {
//        [Fact]
//        public async Task GetAll_ReturnsListOfLeveranciers()
//        {
//            // Arrange
//            var mock = new Mock<ILeverancierController>();

//            mock.Setup(s => s.GetAllAsync())
//                .ReturnsAsync(new List<Leverancier>
//                {
//                    new Leverancier { GebruikerId = 1, Naam = "Test A" },
//                    new Leverancier { GebruikerId = 2, Naam = "Test B" }
//                });

//            var controller = new LeverancierController(mock.Object);

//            // Act
//            var result = await controller.GetAllLeveranciers();

//            // Assert
//            var ok = Assert.IsType<OkObjectResult>(result);
//        }

//    }
//}