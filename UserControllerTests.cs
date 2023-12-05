using CleanArchitecture.Application.Features.UserFeatures.CreateUser;
using CleanArchitecture.Application.Features.UserFeatures.GetAllUser;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.WebAPI.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace YourNamespace.Tests.ControllerTests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkObjectResult()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetAllUserRequest>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<GetAllUserResponse>());

            var loggerMock = new Mock<ILogger<UserController>>();
            var dbContextMock = new Mock<DataContext>();
            var elasticsearchServiceMock = new Mock<IElasticSearchService>();

            var controller = new UserController(mediatorMock.Object, loggerMock.Object, dbContextMock.Object, elasticsearchServiceMock.Object);

            // Act
            var result = await controller.GetAll(CancellationToken.None) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<List<GetAllUserResponse>>(result.Value);
        }

        [Fact]
        public async Task Create_ReturnsOkObjectResult()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest(); // Initialize with required properties

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(createUserRequest, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateUserResponse { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User" });

            var loggerMock = new Mock<ILogger<UserController>>();
            var dbContextMock = new Mock<DataContext>();
            var elasticsearchServiceMock = new Mock<IElasticSearchService>();

            var controller = new UserController(mediatorMock.Object, loggerMock.Object, dbContextMock.Object, elasticsearchServiceMock.Object);
            // Act
            var result = await controller.Create(createUserRequest, CancellationToken.None) as OkObjectResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<CreateUserResponse>(result.Value);
        }

        // Add more test cases for different scenarios as needed
    }
}
