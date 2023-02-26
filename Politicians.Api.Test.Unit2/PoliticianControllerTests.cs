using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using politicz_politicians_and_parties.Contracts.Requests;
using politicz_politicians_and_parties.Contracts.Responses;
using politicz_politicians_and_parties.Controllers;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Result;
using politicz_politicians_and_parties.Services;
using politicz_politicians_and_parties.Validators;
using System.Net;

namespace PoliticiansAndParties.Api.Test.Unit
{
    public class PoliticianControllerTests
    {
        private readonly PoliticianController _sut;
        private readonly IPoliticianService _politicianService = Substitute.For<IPoliticianService>();

        public PoliticianControllerTests()
        {
            _sut = new PoliticianController(_politicianService, new PoliticianRequestValidator());
        }

        [Fact]
        public async Task GetPolitician_ReturnsOkAndObject_WhenPoliticianExists()
        {
            // Arrange
            var politician = new Politician
            {
                Id = 5,
                FrontEndId = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Petr Koller",
                FacebookUrl = "https://facebook.com/testUser",
                TwitterUrl = "https://twitter.com/musk"
            };
            _politicianService.GetAsync(politician.FrontEndId).Returns(new Result<Politician>(politician));

            // Act
            var result = (OkObjectResult)await _sut.GetPolitician(politician.FrontEndId);

            // Assert
            var expectedResponse = politician.ToPoliticianResponse();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetPolitician_ReturnsNotFound_WhenPoliticianDoesNotExist()
        {
            // Arrange
            var notFoundResult = new Result<Politician>(ErrorType.NotFound);
            var id = Guid.NewGuid();

            _politicianService.GetAsync(id).Returns(notFoundResult);

            // Act
            var result = (NotFoundResult)await _sut.GetPolitician(id);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreatePolitician_ReturnsBadRequest_WhenInvalidRequest()
        {
            // Arrange
            var expectedError = new ErrorDetail("Validation error", new Dictionary<string, string[]> { { "FullName", new[] { "'Full Name' must not be empty." } } });

            var politicianRequest = new PoliticianRequest
            {
                BirthDate = DateTime.UtcNow,
                FullName = ""
            };

            // Act
            var result = (BadRequestObjectResult)await _sut.CreatePolitician(Guid.NewGuid(), politicianRequest);

            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.As<ErrorDetail>().Should().BeEquivalentTo(expectedError);

        }

        [Fact]
        public async Task CreatePolitician_ReturnsNotFound_WhenNotFoundResult()
        {
            // Arrange
            var politicianRequest = new PoliticianRequest
            {
                BirthDate = DateTime.UtcNow,
                FullName = "Test name"
            };

            var notFoundResult = new Result<Politician>(ErrorType.NotFound);
            _politicianService.CreateAsync(Arg.Any<Guid>(), Arg.Any<Politician>()).Returns(notFoundResult);

            // Act
            var result = (NotFoundResult)await _sut.CreatePolitician(Guid.NewGuid(), politicianRequest);

            // Assert
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreatePolitician_ReturnsBadRequest_WhenInvalidResult()
        {
            // Arrange
            var politicianRequest = new PoliticianRequest
            {
                BirthDate = DateTime.UtcNow,
                FullName = "Test name"
            };

            var invalidResult = new Result<Politician>(new ErrorDetail("Invalid result"), ErrorType.Invalid);

            _politicianService.CreateAsync(Arg.Any<Guid>(), Arg.Any<Politician>()).Returns(invalidResult);

            // Act
            var result = (BadRequestObjectResult)await _sut.CreatePolitician(Guid.NewGuid(), politicianRequest);

            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.As<ErrorDetail>().Should().BeEquivalentTo(invalidResult.Error);
        }

        [Fact]
        public async Task CreatePolitician_ReturnsStatusCode500_WhenOtherErroResult()
        {
            // Arrange
            var politicianRequest = new PoliticianRequest
            {
                BirthDate = DateTime.UtcNow,
                FullName = "Test name"
            };
            var errorResult = new Result<Politician>(ErrorType.InternalError);
            var partyId = Guid.NewGuid();
            _politicianService.CreateAsync(partyId, Arg.Any<Politician>()).Returns(errorResult);

            // Act
            var result = (StatusCodeResult)await _sut.CreatePolitician(partyId, politicianRequest);

            // Assert
            result.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task CreatePolitician_ReturnsPoliticianResponse_WhenPoliticianCreated()
        {
            // Arrange
            var politicianRequest = new PoliticianRequest
            {
                BirthDate = DateTime.Now,
                FacebookUrl = "https://www.test.com",
                FullName = "Test user",

            };

            var politicianResult = new Result<Politician>(new Politician());

            var partyId = Guid.NewGuid();

            _politicianService.CreateAsync(partyId, Arg.Do<Politician>(x => politicianResult.Data = x)).Returns(politicianResult);

            // Act
            var result = (CreatedAtActionResult)await _sut.CreatePolitician(partyId, politicianRequest);

            // Assert
            var expectedPoliticianResponse = politicianResult.Data!.ToPoliticianResponse();
            result.Value.As<PoliticianResponse>().Should().BeEquivalentTo(expectedPoliticianResponse);
            result.StatusCode.Should().Be(201);
            result.RouteValues!["id"].Should().Be(expectedPoliticianResponse.Id);
        }

        [Fact]
        public async Task UpdatePolitician_ReturnsOkObject_WhenUpdated()
        {
            // Arrange
            var politician = new PoliticianDto
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Testing politician",
            };

            _politicianService.UpdateAsync(politician.Id, politician).Returns(true);

            // Act
            var result = (OkObjectResult)await _sut.UpdatePolitician(politician.Id, politician);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.As<PoliticianDto>().Should().BeEquivalentTo(politician);
        }

        [Fact]
        public async Task UpdatePolitician_ReturnsNotFound_WhenPoliticianDoesNotExist()
        {
            // Arrange
            var politician = new PoliticianDto
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Testing politician",
            };

            _politicianService.UpdateAsync(politician.Id, politician).Returns(false);

            // Act
            var result = (NotFoundResult)await _sut.UpdatePolitician(politician.Id, politician);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeletePolitician_ReturnsOkObject_WhenUpdated()
        {
            // Arrange
            var politicianId = Guid.NewGuid();
            _politicianService.DeleteAsync(politicianId).Returns(true);

            // Arrange
            var result = (OkResult)await _sut.DeletePolitician(politicianId);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeletePolitician_ReturnsNotFound_WhenPoliticianDoesNotExist()
        {
            // Arrange
            var politicianId = Guid.NewGuid();
            _politicianService.DeleteAsync(politicianId).Returns(false);

            // Arrange
            var result = (NotFoundResult)await _sut.DeletePolitician(politicianId);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

    }
}
