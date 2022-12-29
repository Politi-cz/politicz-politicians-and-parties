using politicz_politicians_and_parties.Data;
using politicz_politicians_and_parties.Models;

namespace DotNetDemoProject.Data
{
    public class DbInitializer
    {
        public static void Initialize(PoliticiansPartiesDbContext context) { 
            context.Database.EnsureCreated();

            if (context.PoliticalParties.Any()) {
                return;
            }

            var PoliticalParties = new PoliticalParty[] {
                new PoliticalParty{ Name = "SPD", FrontEndId = Guid.NewGuid()},
                new PoliticalParty{ Name = "ANO", FrontEndId = Guid.NewGuid()}
            };

            foreach (var pp in PoliticalParties) { 
                context.PoliticalParties.Add(pp);
            }

            context.SaveChanges();

            var Politicians = new Politician[] {
                new Politician{ FullName="Tomio", BirthDate = DateTime.Parse("1966-01-01"), PoliticalPartyId = 1, FrontEndId = Guid.NewGuid()},
                new Politician{ FullName="TOnda", BirthDate = DateTime.Parse("1988-01-01"), PoliticalPartyId = 1, FrontEndId = Guid.NewGuid()},
                new Politician{ FullName="Karel", BirthDate = DateTime.Parse("1996-01-01"), PoliticalPartyId = 1, FrontEndId = Guid.NewGuid()},
                new Politician{ FullName="Pavel", BirthDate = DateTime.Parse("2000-01-01"), PoliticalPartyId = 1, FrontEndId = Guid.NewGuid()},
                new Politician{ FullName="Andrej", BirthDate = DateTime.Parse("1962-05-05"), PoliticalPartyId = 2, FrontEndId = Guid.NewGuid()},
                new Politician{ FullName="Karel", BirthDate = DateTime.Parse("1977-04-05"), PoliticalPartyId = 2, FrontEndId = Guid.NewGuid()},
            };

            foreach (var p in Politicians) { 
                context.Politicians.Add(p);
            }

            context.SaveChanges();
        }
    }
}
