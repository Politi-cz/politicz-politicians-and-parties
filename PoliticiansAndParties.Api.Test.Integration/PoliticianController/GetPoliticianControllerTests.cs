using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController
{
    public class GetPoliticianControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
    {
        private readonly HttpClient _client;

        public GetPoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
        }

        [Fact]
        public void Get_ReturnsUser_WhenUserExists() {
           
        }
    }
}
