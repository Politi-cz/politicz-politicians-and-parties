using FluentMigrator;

namespace politicz_politicians_and_parties.Migrations
{
    [Migration(20230112001)]
    public class InitialTables_20230112001 : Migration
    {
        public override void Down()
        {
            Delete.Table("PoliticalParties");
            Delete.Table("Politicians");
            Delete.Table("Tags");
            Delete.Table("PoliticalParties_Tags");
        }

        public override void Up()
        {
            Create.Table("PoliticalParties")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("FrontEndId").AsGuid().NotNullable().Unique()
                .WithColumn("Name").AsString(255).NotNullable().Unique()
                .WithColumn("ImageUrl").AsString(255).NotNullable();

            Create.Table("Politicians")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("FrontEndId").AsGuid().NotNullable().Unique()
                .WithColumn("BirthDate").AsDateTime2().NotNullable()
                .WithColumn("FullName").AsString(255).NotNullable()
                .WithColumn("InstagramUrl").AsString(255).Nullable()
                .WithColumn("TwitterUrl").AsString(255).Nullable()
                .WithColumn("FacebookUrl").AsString(255).Nullable()
                .WithColumn("PoliticalPartyId").AsInt32().NotNullable();

            Create.Table("Tags")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique();

            Create.Table("PoliticalParties_Tags")
                .WithColumn("PoliticalPartyId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("TagId").AsInt32().NotNullable().PrimaryKey();


            Create.ForeignKey()
                .FromTable("Politicians").ForeignColumn("PoliticalPartyId")
                .ToTable("PoliticalParties").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            Create.Index()
                .OnTable("Politicians").OnColumn("PoliticalPartyId").Ascending();
        }
    }
}
