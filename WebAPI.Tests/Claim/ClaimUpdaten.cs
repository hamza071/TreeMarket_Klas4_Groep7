//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TreeMarket_Klas4_Groep7.Controllers;
//using TreeMarket_Klas4_Groep7.Models.DTO;
//using TreeMarket_Klas4_Groep7.Services;
//using TreeMarket_Klas4_Groep7.Interfaces;
//using TreeMarket_Klas4_Groep7.Models;

//namespace WebAPI.Tests.TClaim
//{
//    public class ClaimUpdaten
//    {
//        [Fact]
//        public async Task UpdateClaim_ReturnsNoContent_WhenUpdated()
//        {
//            var mock = new Mock<IClaimController>();
//            mock.Setup(s => s.UpdateAsync(1, It.IsAny<Claim>()))
//                .ReturnsAsync(true);

//            var controller = new ClaimController(mock.Object);

//            var result = await controller.PutClaim(1, new Claim());

//            Assert.IsType<NoContentResult>(result);
//        }

//    }
//}