using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Services;
using politicz_politicians_and_parties.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politicians.Api.Test.Unit
{
    public class PoliticianServiceTests
    {
        private readonly PoliticianService _sut;
        private readonly IPoliticianRepository _politicianRepository = Substitute.For<IPoliticianRepository>();
        private readonly IPoliticalPartyRepository _politicalPartyRepository= Substitute.For<IPoliticalPartyRepository>();

        public PoliticianServiceTests()
        {
            _sut = new PoliticianService(_politicianRepository, _politicalPartyRepository, new PoliticianDtoValidator());
        }

        [Fact]
        public async Task GetPoliticianAsync_ShouldReturnPoliticianDto_WhenPoliticianExists() {
            // Arrange
            var existingPolitician = new Politician {
                Id = 1,
                FrontEndId = Guid.NewGuid(),
                BirthDate = DateTime.Now,
                FullName = "Petr Koller",
                PoliticalPartyId = 25,
            };
            var expectedPolitician = existingPolitician.ToPoliticianDto();
            _politicianRepository.GetPoliticianAsync(existingPolitician.FrontEndId).Returns(existingPolitician);

            // Act
            var result = await _sut.GetPoliticianAsync(existingPolitician.FrontEndId);

            // Assert
            result.Should().BeEquivalentTo(expectedPolitician);
        }

        [Fact]
        public async Task GetPoliticianAsync_ShouldReturnNull_WhenPoliticianDoesNotExist() { 
            // Arrange
            _politicianRepository.GetPoliticianAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = await _sut.GetPoliticianAsync(Arg.Any<Guid>());

            // Assert
            result.Should().BeNull();
        }
    }
}
