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
//using TreeMarket_Klas4_Groep7.Services;

//namespace WebAPI.Tests.TDashboard
//{
//    public class DashboardTestRead
//    {
//        [Fact]
//        public async Task GetById_ReturnsOk_WhenFound()
//        {
//            var mock = new Mock<IDashboardController>();
//            mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Dashboard { DashboardID = 1 });

//            var controller = new DashboardsController(mock.Object);

//            var response = await controller.GetById(1);

//            Assert.IsType<OkObjectResult>(response);
//        }
//    }
//}