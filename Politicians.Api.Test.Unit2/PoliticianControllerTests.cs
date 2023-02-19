using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Controllers;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Services;
using System.Net;

namespace PoliticiansAndParties.Api.Test.Unit
{
    public class PoliticianControllerTests
    {
        private readonly PoliticianController _sut;
        private readonly IPoliticianService _politicianService = Substitute.For<IPoliticianService>();

        public PoliticianControllerTests()
        {
            _sut = new PoliticianController(_politicianService);
        }

        [Fact]
        public async Task GetPolitician_ReturnsOkAndObject_WhenPoliticianExists()
        {
            // Arrange
            var politician = new PoliticianDto
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Petr Koller",
                FacebookUrl = "https://facebook.com/testUser",
                TwitterUrl = "https://twitter.com/musk"
            };
            _politicianService.GetAsync(politician.Id).Returns(politician);

            // Act
            var result = (OkObjectResult)await _sut.GetPolitician(politician.Id);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().BeEquivalentTo(politician);
        }

        [Fact]
        public async Task GetPolitician_ReturnsNotFound_WhenPoliticianDoesNotExist()
        {
            // Arrange
            _politicianService.GetAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = (NotFoundResult)await _sut.GetPolitician(Arg.Any<Guid>());

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreatePolitician_ShouldReturnInternalServerError_WhenPoliticianNotCreated()
        {
            // Arrange
            _politicianService.CreateAsync(Arg.Any<Guid>(), Arg.Any<PoliticianDto>()).Returns(false);

            // Act
            var result = (StatusCodeResult)await _sut.CreatePolitician(Arg.Any<Guid>(), Arg.Any<PoliticianDto>());

            // Assert
            result.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task CreatePolitician_ShouldReturnPolitician_WhenPoliticianCreated()
        {
            // Arrange
            var politicianDto = new PoliticianDto
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Testing politician",
            };

            var partyId = Guid.NewGuid();
            var expectedPoliticianDto = new PoliticianDto();

            _politicianService.CreateAsync(partyId, Arg.Do<PoliticianDto>(x => expectedPoliticianDto = x)).Returns(true);

            // Act
            var result = (CreatedAtActionResult)await _sut.CreatePolitician(partyId, politicianDto);

            // Assert
            result.Value.As<PoliticianDto>().Should().BeEquivalentTo(expectedPoliticianDto);
            result.StatusCode.Should().Be(201);
            result.RouteValues!["id"].Should().Be(expectedPoliticianDto.Id);
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
