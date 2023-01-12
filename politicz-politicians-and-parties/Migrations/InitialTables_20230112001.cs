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
        /*CREATE TABLE [dbo].[PoliticalParties](
            [Id] [int] IDENTITY(1,1) NOT NULL,
            [FrontEndId] [uniqueidentifier] NOT NULL,
            [Name] [nvarchar](max) NOT NULL,
            [ImageUrl] [nvarchar](max) NOT NULL,
         CONSTRAINT [PK_PoliticalParties] PRIMARY KEY CLUSTERED */
        public override void Up()
        {
            Create.Table("PoliticalParties")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("FrontEndId").AsGuid().NotNullable().Unique()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("ImageUrl").AsString(255).NotNullable();
        }
    }
}
