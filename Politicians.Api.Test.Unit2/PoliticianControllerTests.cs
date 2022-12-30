using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Controllers;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Politicians.Api.Test.Unit
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
        public async Task GetPolitician_ReturnsOkAndObject_WhenPoliticianExists() {
            // TODO Change PoliticianDto and add Id attributeff
            // Arrange
            var id = Guid.NewGuid();
            var politician = new PoliticianDto
            {
                BirthDate = DateTime.Now,
                FullName = "Petr Koller",
                FacebookUrl = "https://facebook.com/testUser",
                TwitterUrl = "https://twitter.com/musk"
            };
            _politicianService.GetPoliticianAsync(id).Returns(politician);

            // Act
            var result = (OkObjectResult)await _sut.GetPolitician(id);

            // Assert
            // TODO Add constants instead of number
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(politician);
        }

        [Fact]
        public async Task GetPolitician_ReturnsNotFound_WhenPoliticianDoesNotExist()
        {
            // Arrange
            _politicianService.GetPoliticianAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = (NotFoundResult)await _sut.GetPolitician(Arg.Any<Guid>());

            // Assert
            // TODO Add constants instead of number
            result.StatusCode.Should().Be(404);
        }
    }
}
