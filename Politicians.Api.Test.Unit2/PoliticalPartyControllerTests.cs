﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using politicz_politicians_and_parties.Controllers;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Services;
using System.Net;

namespace Politicians.Api.Test.Unit
{
    public class PoliticalPartyControllerTests
    {
        private readonly PoliticalPartyController _sut;
        private readonly IPoliticalPartyService _politicalPartyService = Substitute.For<IPoliticalPartyService>();

        public PoliticalPartyControllerTests()
        {
            _sut = new PoliticalPartyController(_politicalPartyService);
        }



        [Fact]
        public async Task GetPoliticalParty_ReturnsOkObject_WhenPartyExists()
        {
            // Arrange
            var politicalPartyDto = new PoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://testingurl.com",
                Name = "KAKAKA",
                Politicians = new List<PoliticianDto> {
                    new PoliticianDto{
                        Id= Guid.NewGuid(),
                        BirthDate= DateTime.Now,
                        FullName = "Karel Jejda",
                        TwitterUrl = "twtr.com"
                    },
                    new PoliticianDto{
                        Id= Guid.NewGuid(),
                        BirthDate= new DateTime(2000,11,30),
                        FullName = "John Bestot",
                        InstagramUrl = "ig.com"
                    }
                }
            };

            _politicalPartyService.GetOneAsync(politicalPartyDto.Id).Returns(politicalPartyDto);

            // Act

            var result = (OkObjectResult)await _sut.GetPoliticalParty(politicalPartyDto.Id);

            // Assert
            result.Value.Should().BeEquivalentTo(politicalPartyDto);
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetPoliticalParty_ReturnsNotFound_WhenPartyDesNotExist()
        {
            // Arrange
            _politicalPartyService.GetOneAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act

            var result = (NotFoundResult)await _sut.GetPoliticalParty(Arg.Any<Guid>());

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPoliticalParties_ReturnsOkObject_WhenPartyExists()
        {
            // Arrange
            IEnumerable<PoliticalPartySideNavDto> politicalPartiesSideNav = new List<PoliticalPartySideNavDto>() {
                new PoliticalPartySideNavDto{
                    Id = Guid.NewGuid(),
                    Name= "Test",
                    ImageUrl = "img.com"
                },
                new PoliticalPartySideNavDto{
                    Id= Guid.NewGuid(),
                    Name= "Test2",
                    ImageUrl = "randomUIrl.com"
                }
            };

            _politicalPartyService.GetAllAsync().Returns(politicalPartiesSideNav);

            // Act
            var result = (OkObjectResult)await _sut.GetPoliticalParties();

            // Assert
            result.Value.Should().BeEquivalentTo(politicalPartiesSideNav);
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

        }

        [Fact]
        public async Task GetPoliticalParties_ReturnsOkEmptyObject_WhenPartyDoesNotExist()
        {
            // Arrange
            _politicalPartyService.GetAllAsync().Returns(Enumerable.Empty<PoliticalPartySideNavDto>());

            // Act
            var result = (OkObjectResult)await _sut.GetPoliticalParties();

            // Assert
            result.Value.Should().BeEquivalentTo(Enumerable.Empty<PoliticalPartySideNavDto>());
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreatePoliticalParty_ShouldReturnPoliticalParty_WhenPoliticalPartyCreated()
        {
            // Arrange
            var politicalPartyDto = new PoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                Name = "New party",
                ImageUrl = "https://imageurl.com",
                Tags = new HashSet<string> { "Test party" },
                Politicians = new List<PoliticianDto> {
                    new PoliticianDto {
                        BirthDate= DateTime.Now,
                        FullName = "Testing politician",
                    }
                }
            };

            var expectedParty = new PoliticalPartyDto();

            _politicalPartyService.CreateAsync(Arg.Do<PoliticalPartyDto>(x => expectedParty = x)).Returns(true);

            // Act
            var result = (CreatedAtActionResult)await _sut.CreatePoliticalParty(politicalPartyDto);

            // Assert
            result.Value.As<PoliticalPartyDto>().Should().BeEquivalentTo(expectedParty);
            result.StatusCode.Should().Be(201);
            result.RouteValues!["id"].Should().Be(expectedParty.Id);
        }

        [Fact]
        public async Task CreatePoliticalParty_ShouldReturnInternalServerError_WhenPoliticalPartyNotCreated()
        {
            // Arrange
            _politicalPartyService.CreateAsync(Arg.Any<PoliticalPartyDto>()).Returns(false);

            // Act
            var result = (StatusCodeResult)await _sut.CreatePoliticalParty(Arg.Any<PoliticalPartyDto>());

            // Assert
            result.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task UpdatePoliticalParty_ReturnsOkObject_WhenUpdated()
        {
            // Arrange
            var politicalParty = new UpdatePoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://test.com",
                Name = "Test",
                Tags = new HashSet<string> { "random" }
            };

            _politicalPartyService.UpdateAsync(politicalParty).Returns(true);

            // Act
            var result = (OkObjectResult)await _sut.UpdatePoliticalParty(politicalParty.Id, politicalParty);

            // Arrange
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.As<UpdatePoliticalPartyDto>().Should().BeEquivalentTo(politicalParty);
        }

        [Fact]
        public async Task UpdatePoliticalParty_ReturnsNotFound_WhenPartyDoesNotExist()
        {
            // Arrange
            var politicalParty = new UpdatePoliticalPartyDto
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://test.com",
                Name = "Test",
                Tags = new HashSet<string> { "random" }
            };

            _politicalPartyService.UpdateAsync(politicalParty).Returns(false);

            // Act
            var result = (NotFoundResult)await _sut.UpdatePoliticalParty(politicalParty.Id, politicalParty);

            // Arrange
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeletePoliticalParty_ReturnsOkObject_WhenUpdated()
        {
            // Arrange
            var partyId = Guid.NewGuid();
            _politicalPartyService.DeleteAsync(partyId).Returns(true);

            // Arrange
            var result = (OkResult)await _sut.DeletePoliticalParty(partyId);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeletePoliticalParty_ReturnsNotFound_WhenPartyDoesNotExist()
        {
            // Arrange
            var partyId = Guid.NewGuid();
            _politicalPartyService.DeleteAsync(partyId).Returns(false);

            // Arrange
            var result = (NotFoundResult)await _sut.DeletePoliticalParty(partyId);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

    }
}
