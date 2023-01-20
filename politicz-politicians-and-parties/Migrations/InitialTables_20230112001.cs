using FluentMigrator;
using FluentMigrator.SqlServer;

namespace politicz_politicians_and_parties.Migrations
{
    [Migration(20230112001)]
    public class InitialTables_20230112001 : Migration
    {
        public override void Down()
        {
            Delete.Table("Politicians");
            Delete.Table("PoliticalParties");
        }

        public override void Up()
        {
            Create.Table("PoliticalParties")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("FrontEndId").AsGuid().NotNullable().Unique()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("ImageUrl").AsString(255).NotNullable();

            Create.Table("Politicians")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("FrontEndId").AsGuid().NotNullable().Unique()
                .WithColumn("BirthDate").AsDateTime().NotNullable()
                .WithColumn("FullName").AsString(255).NotNullable()
                .WithColumn("InstagramUrl").AsString(255).Nullable()
                .WithColumn("TwitterUrl").AsString(255).Nullable()
                .WithColumn("FacebookUrl").AsString(255).Nullable()
                .WithColumn("PoliticalPartyId").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("Politicians").ForeignColumn("PoliticalPartyId")
                .ToTable("PoliticalParties").PrimaryColumn("Id");

            Create.Index()
                .OnTable("Politicians").OnColumn("PoliticalPartyId").Ascending().WithOptions().NonClustered();
        }
    }
}
