using CleanArchitecture.Application.Features.UserFeatures.CreateUser;
using CleanArchitecture.Application.Features.UserFeatures.GetAllUser;
using CleanArchitecture.Application.Repositories;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Persistence;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.WebAPI.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Tests.ControllerTests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsActionResult()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetAllUserRequest>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<GetAllUserResponse>());

            var loggerMock = new Mock<ILogger<UserController>>();

            // Mocking DbContextOptions
            var options = new DbContextOptionsBuilder<DataContext>()
                // .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Creating the DataContext with mocked options
            var dbContextMock = new Mock<DataContext>(options);

            // Mocking dependencies for the UserController
            var elasticsearchServiceMock = new Mock<IElasticSearchService>();

            var controller = new UserController(mediatorMock.Object, loggerMock.Object, dbContextMock.Object, elasticsearchServiceMock.Object);

            // Act
            var result = await controller.GetAll(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<GetAllUserResponse>>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsActionResult()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest("test@example.com", "Test User");

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(createUserRequest, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateUserResponse { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User" });

            var loggerMock = new Mock<ILogger<UserController>>();

            // Mocking DbContextOptions
            var options = new DbContextOptionsBuilder<DataContext>()
                //.UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Creating the DataContext with mocked options
            var dbContextMock = new Mock<DataContext>(options);
            // Mocking dependencies for the UserController
            var elasticsearchServiceMock = new Mock<IElasticSearchService>();

            var controller = new UserController(mediatorMock.Object, loggerMock.Object, dbContextMock.Object, elasticsearchServiceMock.Object);

            // Act
            var result = await controller.Create(createUserRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<CreateUserResponse>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public async Task SearchDocuments_ReturnsActionResult()
        {
            // Arrange
            var searchTerm = "test";
            var expectedDocuments = new List<Document>
            {
                new Document { Id = "1", Content = "Test Content 1" },
                new Document { Id = "2", Content = "Test Content 2" }
            };

            var elasticsearchServiceMock = new Mock<IElasticSearchService>();
            elasticsearchServiceMock
    .Setup(x => x.SearchDocuments(searchTerm))
    .ReturnsAsync((IEnumerable<Document>)expectedDocuments);
            var loggerMock = new Mock<ILogger<UserController>>();
            var controller = new UserController(null, loggerMock.Object, null, elasticsearchServiceMock.Object);

            // Act
            var result = await controller.SearchDocuments(searchTerm);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Document>>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var documents = Assert.IsAssignableFrom<IEnumerable<Document>>(objectResult.Value);
            Assert.Equal(expectedDocuments.Count, documents.Count());
        }
        //    [Fact]
        //    public async Task SearchDocuments_ReturnsActionResult()
        //    {
        //        // Arrange
        //        var searchTerm = "test";
        //        var expectedDocuments = new List<System.Reflection.Metadata.Document>
        //{
        //    new System.Reflection.Metadata.Document(),
        //    new System.Reflection.Metadata.Document()
        //    // Add properties initialization here as needed
        //};

        //        var elasticsearchServiceMock = new Mock<IElasticSearchService>();
        //        elasticsearchServiceMock
        //            .Setup(x => x.SearchDocuments(searchTerm))
        //            .ReturnsAsync(expectedDocuments);

        //        var loggerMock = new Mock<ILogger<UserController>>();
        //        var controller = new UserController(null, loggerMock.Object, null, elasticsearchServiceMock.Object);

        //        // Act
        //        var result = await controller.SearchDocuments(searchTerm);

        //        // Assert
        //        var actionResult = Assert.IsType<ActionResult<System.Collections.Generic.IEnumerable<System.Reflection.Metadata.Document>>>(result);
        //        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        //        var documents = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<System.Reflection.Metadata.Document>>(objectResult.Value);
        //        Assert.Equal(expectedDocuments.Count, documents.Count());
        //    }
        //[Fact]
        //public async Task SearchDocuments_ReturnsActionResult()
        //{
        //    // Arrange
        //    var searchTerm = "test";
        //    var expectedDocuments = new List<Document>
        //    {
        //        new Document { Id = "1", Content = "Test Content 1" },
        //        new Document { Id = "2", Content = "Test Content 2" }
        //    };
        //    var elasticsearchServiceMock = new Mock<IElasticSearchService>();
        //    Moq.Language.Flow.ISetup<IElasticSearchService, Task<IEnumerable<System.Reflection.Metadata.Document>>> setup = elasticsearchServiceMock
        //                    .Setup(x => x.SearchDocuments(searchTerm));
        //    var loggerMock = new Mock<ILogger<UserController>>();
        //    var controller = new UserController(null, loggerMock.Object, null, elasticsearchServiceMock.Object);
        //    // Act
        //    var result = await controller.SearchDocuments(searchTerm);
        //    // Assert
        //    var actionResult = Assert.IsType<ActionResult<Document>>(result);
        //    var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        //    var documents = Assert.IsAssignableFrom<IEnumerable<Document>>(objectResult.Value);
        //    Assert.Equal(expectedDocuments.Count, documents.Count());
        //}
    }
}
