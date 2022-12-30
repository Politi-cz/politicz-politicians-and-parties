using Microsoft.EntityFrameworkCore;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PoliticalParty>(entity => entity.HasIndex(e => e.FrontEndId).IsClustered(false));
            modelBuilder.Entity<Politician>(entity => entity.HasIndex(e => e.FrontEndId).IsClustered(false));
        }

        public DbSet<PoliticalParty> PoliticalParties { get; set; }
        public DbSet<Politician> Politicians { get; set; }
    }
}
