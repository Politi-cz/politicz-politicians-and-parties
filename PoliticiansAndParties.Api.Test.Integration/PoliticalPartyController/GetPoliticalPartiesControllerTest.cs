using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticalPartyController
{
    public class GetPoliticalPartiesControllerTest : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        // TODO FIll in test cases when seeding is removed and creating parties added
        [Fact]
        public async Task GetPoliticalParties_ReturnsPoliticalParties_WhenPartiesExist()
        {
            // Arrange

            // Act
            // Assert

        }

        [Fact]
        public async Task GetPoliticalParties_ReturnsEmptyList_WhenNoPoliticalPartyExist()
        {
            // Arrange
            // Act

            // Assert

        }
    }
}
