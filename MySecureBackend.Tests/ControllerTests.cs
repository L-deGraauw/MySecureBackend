using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Moq;
using MySecureBackend.WebApi.Controllers;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.Tests
{
    [TestClass]
    public sealed class ControllerTests
    {
        private ExampleObjectsController controller;
        private Mock<IExampleObjectRepository> exampleObjectRepository;
        private Mock<IAuthenticationService> authenticationService;

        [TestInitialize]
        public void Setup()
        {
            exampleObjectRepository = new Mock<IExampleObjectRepository>();
            authenticationService = new Mock<IAuthenticationService>();

            controller = new ExampleObjectsController(exampleObjectRepository.Object, authenticationService.Object);
        }

        [TestMethod]
        public async Task Get_ExampleObjectThatDoesNotExist_Returns404NotFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            exampleObjectRepository.Setup(x => x.SelectAsync(id)).ReturnsAsync(null as ExampleObject);

            // Act
            var response = await controller.GetByIdAsync(id);

            // Assert
            Assert.IsInstanceOfType<NotFoundObjectResult>(response.Result);
        }


            [TestMethod]
            public async Task Create_SameUser_CannotCreateDuplicateEnvironmentNames()
            {
                // Arrange
                var envRepo = new Mock<IEnvironment2DRepository>();
                var authService = new Mock<IAuthenticationService>();
                var controller = new MySecureBackend.WebApi.Controllers.Environment2DController(envRepo.Object, authService.Object);

                var userId = Guid.NewGuid().ToString();
                authService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);

                var store = new List<Environment2D>();

                envRepo.Setup(r => r.ExistsByOwnerAndNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                       .ReturnsAsync((string owner, string name) => store.Any(e => e.OwnerUserId == owner && e.Name == name));

                envRepo.Setup(r => r.CountByOwnerAsync(It.IsAny<string>()))
                       .ReturnsAsync((string owner) => store.Count(e => e.OwnerUserId == owner));

                envRepo.Setup(r => r.InsertAsync(It.IsAny<Environment2D>()))
                       .Callback<Environment2D>(e => store.Add(e))
                       .ReturnsAsync((Environment2D e) => e.Id);

                envRepo.Setup(r => r.SelectAsync()).ReturnsAsync(() => store.AsEnumerable());

                // Act - two attempts with same name
                var firstResponse = await controller.CreateAsync(new Environment2D { Name = "TestWorld" });
                var secondResponse = await controller.CreateAsync(new Environment2D { Name = "TestWorld" });

                // Assert - first creation returns Created, second returns Conflict
                Assert.IsInstanceOfType(firstResponse.Result, typeof(CreatedAtRouteResult));
                Assert.IsInstanceOfType(secondResponse.Result, typeof(ConflictObjectResult));

                // There should be only one stored environment with that name for the user
                var count = store.Count(e => e.OwnerUserId == userId && e.Name == "TestWorld");
                Assert.AreEqual(1, count);

                // Verify InsertAsync was called only once
                envRepo.Verify(r => r.InsertAsync(It.IsAny<Environment2D>()), Times.Once);
            }

        [TestMethod]
        public async Task Create_MaxEnvironmentsReached_ReturnsConflict()
        {
            // Arrange
            var envRepo = new Mock<IEnvironment2DRepository>();
            var authService = new Mock<IAuthenticationService>();
            var controller = new Environment2DController(envRepo.Object, authService.Object);

            var userId = Guid.NewGuid().ToString();
            authService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            envRepo.Setup(r => r.CountByOwnerAsync(userId)).ReturnsAsync(5);

            // Act
            var response = await controller.CreateAsync(new Environment2D { Name = "TestWorld" });

            // Assert
            Assert.IsInstanceOfType<ConflictObjectResult>(response.Result);
        }

        [TestMethod]
        public async Task Delete_OwnEnvironment_ReturnsOk()
        {
            // Arrange
            var envRepo = new Mock<IEnvironment2DRepository>();
            var authService = new Mock<IAuthenticationService>();
            var controller = new Environment2DController(envRepo.Object, authService.Object);

            var userId = Guid.NewGuid().ToString();
            var id = Guid.NewGuid();
            authService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            envRepo.Setup(r => r.SelectAsync(id)).ReturnsAsync(new Environment2D { Id = id, Name = "TestWorld", OwnerUserId = userId });
            envRepo.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var response = await controller.DeleteAsync(id);

            // Assert
            Assert.IsInstanceOfType<OkResult>(response);
        }

        [TestMethod]
        public async Task Delete_OtherUsersEnvironment_ReturnsNotFound()
        {
            // Arrange
            var envRepo = new Mock<IEnvironment2DRepository>();
            var authService = new Mock<IAuthenticationService>();
            var controller = new Environment2DController(envRepo.Object, authService.Object);

            var userId = Guid.NewGuid().ToString();
            var otherUserId = Guid.NewGuid().ToString();
            var id = Guid.NewGuid();
            authService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            envRepo.Setup(r => r.SelectAsync(id)).ReturnsAsync(new Environment2D { Id = id, Name = "TestWorld", OwnerUserId = otherUserId });

            // Act
            var response = await controller.DeleteAsync(id);

            // Assert
            Assert.IsInstanceOfType<NotFoundObjectResult>(response);
            envRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}