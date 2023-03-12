namespace PoliticiansAndParties.Api.Migrations;

[Migration(20230112001)]
public class InitialTables_20230112001 : Migration
{
    public override void Down()
    {
        _ = Delete.Table("PoliticalParties");
        _ = Delete.Table("Politicians");
        _ = Delete.Table("Tags");
        _ = Delete.Table("PoliticalParties_Tags");
    }

    public override void Up()
    {
        // TODO: In the future might use fluent migrator only for keeping track of version and only execute prepared scripts with tables with Execute.Script instead of defining the tables here in code
        // TODO: Instead of defining SQL in Repositories create procedures which will be part of the create script.
        _ = Create.Table("PoliticalParties")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("FrontEndId").AsGuid().NotNullable().Unique()
            .WithColumn("Name").AsString(255).NotNullable().Unique()
            .WithColumn("ImageUrl").AsString(255).NotNullable();

        _ = Create.Table("Politicians")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("FrontEndId").AsGuid().NotNullable().Unique()
            .WithColumn("BirthDate").AsDateTime2().NotNullable()
            .WithColumn("ImageUrl").AsString(255).NotNullable()
            .WithColumn("FullName").AsString(255).NotNullable()
            .WithColumn("InstagramUrl").AsString(255).Nullable()
            .WithColumn("TwitterUrl").AsString(255).Nullable()
            .WithColumn("FacebookUrl").AsString(255).Nullable()
            .WithColumn("PoliticalPartyId").AsInt32().NotNullable();

        _ = Create.Table("Tags")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(255).NotNullable().Unique();

        _ = Create.Table("PoliticalParties_Tags")
            .WithColumn("PoliticalPartyId").AsInt32().NotNullable().PrimaryKey()
            .WithColumn("TagId").AsInt32().NotNullable().PrimaryKey();

        _ = Create.ForeignKey()
            .FromTable("Politicians").ForeignColumn("PoliticalPartyId")
            .ToTable("PoliticalParties").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        _ = Create.Index()
            .OnTable("Politicians").OnColumn("PoliticalPartyId").Ascending();
    }
}
