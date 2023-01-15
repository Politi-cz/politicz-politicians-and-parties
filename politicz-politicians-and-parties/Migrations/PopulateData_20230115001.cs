using FluentMigrator;
using FluentMigrator.SqlServer;
using System.Collections.Generic;

namespace politicz_politicians_and_parties.Migrations
{
    [Migration(20230115001)]
    public class PopulateData_20230115001 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("PoliticalParties")
                .Row(new { Id = 1, FrontEndId = new Guid("08e8fb6e-b166-4ec4-ae4e-6b3096f84e1b"), Name = "SPD", ImageUrl = "https://www.spd.cz/wp-content/uploads/ke_stazeni/grafika/stredni-logo-spd.png" })
                .Row(new { Id = 2, FrontEndId = new Guid("24ed715a-31cb-4fca-9726-533072b0c79d"), Name = "ANO", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/92/ANO_Logo.svg/1200px-ANO_Logo.svg.png" })
                .Row(new { Id = 3, FrontEndId = new Guid("bfc3975f-e3bb-4511-bd24-88c9e0f457b1"), Name = "ODS", ImageUrl = "https://www.ods.cz/img/logo/ods-logo-prechod.jpg" });

            Delete.FromTable("Politicians")
                .Row(new { Id = 1, FrontEndId = new Guid("a5e15559-ebba-426a-8a38-f56e3421903c"), FullName = "Tomio", BirthDate = new DateTime(1966, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 2, FrontEndId = new Guid("734dfb56-5870-4487-aae2-12b207f0bcd2"), FullName = "TOnda", BirthDate = new DateTime(1988, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 3, FrontEndId = new Guid("8cf61bec-bd1d-4227-aa9e-76338e2fea2c"), FullName = "Karel", BirthDate = new DateTime(1966, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 4, FrontEndId = new Guid("996b175f-2cff-40a5-b568-0fbc74328a66"), FullName = "Pavel", BirthDate = new DateTime(2000, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 5, FrontEndId = new Guid("4756c20d-826f-4c7c-89d0-3252e130546d"), FullName = "Andrej", BirthDate = new DateTime(1962, 05, 05), PoliticalPartyId = 2 })
                .Row(new { Id = 6, FrontEndId = new Guid("65e86328-5f07-45b6-b5fe-4e6be3d98d2f"), FullName = "Karel", BirthDate = new DateTime(1977, 04, 05), PoliticalPartyId = 2 });
        }

        public override void Up()
        {
            Insert.IntoTable("PoliticalParties")
                .WithIdentityInsert()
                .Row(new { Id = 1, FrontEndId = new Guid("08e8fb6e-b166-4ec4-ae4e-6b3096f84e1b"), Name = "SPD", ImageUrl = "https://www.spd.cz/wp-content/uploads/ke_stazeni/grafika/stredni-logo-spd.png" })
                .Row(new { Id = 2, FrontEndId = new Guid("24ed715a-31cb-4fca-9726-533072b0c79d"), Name = "ANO", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/92/ANO_Logo.svg/1200px-ANO_Logo.svg.png" })
                .Row(new { Id = 3, FrontEndId = new Guid("bfc3975f-e3bb-4511-bd24-88c9e0f457b1"), Name = "ODS", ImageUrl = "https://www.ods.cz/img/logo/ods-logo-prechod.jpg" });

            Insert.IntoTable("Politicians")
                .WithIdentityInsert()
                .Row(new { Id = 1, FrontEndId = new Guid("a5e15559-ebba-426a-8a38-f56e3421903c"), FullName = "Tomio", BirthDate = new DateTime(1966, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 2, FrontEndId = new Guid("734dfb56-5870-4487-aae2-12b207f0bcd2"), FullName = "TOnda", BirthDate = new DateTime(1988, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 3, FrontEndId = new Guid("8cf61bec-bd1d-4227-aa9e-76338e2fea2c"), FullName = "Karel", BirthDate = new DateTime(1966, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 4, FrontEndId = new Guid("996b175f-2cff-40a5-b568-0fbc74328a66"), FullName = "Pavel", BirthDate = new DateTime(2000, 01, 01), PoliticalPartyId = 1 })
                .Row(new { Id = 5, FrontEndId = new Guid("4756c20d-826f-4c7c-89d0-3252e130546d"), FullName = "Andrej", BirthDate = new DateTime(1962, 05, 05), PoliticalPartyId = 2 })
                .Row(new { Id = 6, FrontEndId = new Guid("65e86328-5f07-45b6-b5fe-4e6be3d98d2f"), FullName = "Karel", BirthDate = new DateTime(1977, 04, 05), PoliticalPartyId = 2 });
        }
    }

    /*SET IDENTITY_INSERT[dbo].[PoliticalParties]
    ON

INSERT[dbo].[PoliticalParties]
    ([Id], [FrontEndId], [Name], [ImageUrl]) VALUES(1, N'08e8fb6e-b166-4ec4-ae4e-6b3096f84e1b', N'SPD', N'')

INSERT[dbo].[PoliticalParties]
    ([Id], [FrontEndId], [Name], [ImageUrl]) VALUES(2, N'24ed715a-31cb-4fca-9726-533072b0c79d', N'ANO', N'')

SET IDENTITY_INSERT[dbo].[PoliticalParties]
    OFF
SET IDENTITY_INSERT[dbo].[Politicians]
    ON

INSERT[dbo].[Politicians] ([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(1, N'a5e15559-ebba-426a-8a38-f56e3421903c', N'Tomio', CAST(N'1966-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians] ([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(2, N'734dfb56-5870-4487-aae2-12b207f0bcd2', N'TOnda', CAST(N'1988-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians] ([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(3, N'8cf61bec-bd1d-4227-aa9e-76338e2fea2c', N'Karel', CAST(N'1996-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians] ([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(4, N'996b175f-2cff-40a5-b568-0fbc74328a66', N'Pavel', CAST(N'2000-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians] ([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(5, N'4756c20d-826f-4c7c-89d0-3252e130546d', N'Andrej', CAST(N'1962-05-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 2)

INSERT[dbo].[Politicians] ([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(6, N'65e86328-5f07-45b6-b5fe-4e6be3d98d2f', N'Karel', CAST(N'1977-04-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 2)*/
}
