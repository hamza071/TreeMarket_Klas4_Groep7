//using Xunit;
//using WebAPI.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;

//namespace WebAPI.Tests
//{
//    public class GebruikerControllerTests
//    {
//        [Fact]
//        public void GetAll_ReturnsOkResult_WithListOfUsers()
//        {
//            // Arrange
//            var controller = new GebruikerController();

//            // Act
//            var result = controller.GetAll();

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var users = Assert.IsType<List<string>>(okResult.Value); // pas type aan naar jouw return type
//            Assert.NotEmpty(users);
//        }
//    }
//}
