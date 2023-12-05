using CleanArchitecture.Application.Features.UserFeatures.CreateUser;
using CleanArchitecture.Application.Features.UserFeatures.GetAllUser;
using CleanArchitecture.Application.Repositories;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Persistence;
using CleanArchitecture.Persistence.Context;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.WebAPI.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _dbContext;
        private readonly IElasticSearchService _elasticsearchService;

        public UserController(IMediator mediator, ILogger<UserController> logger, DataContext dbContext, IElasticSearchService elasticsearchService)
        {
            _mediator = mediator;
            _logger = logger;
            _dbContext = dbContext;
            _elasticsearchService = elasticsearchService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllUserResponse>>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetAll method called.");
                var response = await _mediator.Send(new GetAllUserRequest(), cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAll method.");
                await LogExceptionAsync(ex);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CreateUserResponse>> Create(CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Create method called.");
                this._elasticsearchService.AddDocument(new Document { Content = request.Email, Id = Guid.NewGuid().ToString() });
                var response = await _mediator.Send(request, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Create method.");
                await LogExceptionAsync(ex);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<Document>> SearchDocuments(string searchTerm)
        {
            try
            {
                _logger.LogInformation("SearchDocuments method called.");
                var searchResults = await _elasticsearchService.SearchDocuments(searchTerm);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in SearchDocuments method.");
                return StatusCode(500, "Internal server error.");
            }
        }

        private async Task LogExceptionAsync(Exception exception)
        {
            var logEntry = new Log
            {
                LogLevel = "Error",
                Timestamp = DateTime.UtcNow,
                Message = exception.Message,
                Exception = exception.ToString()
            };
            _dbContext.Logs.Add(logEntry);
            await _dbContext.SaveChangesAsync();
        }
    }
}
