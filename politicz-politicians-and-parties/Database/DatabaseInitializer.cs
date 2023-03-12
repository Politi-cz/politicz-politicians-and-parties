using Dapper;

namespace politicz_politicians_and_parties.Database
{
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync(string dbName)
        {
            var query = "SELECT * FROM sys.databases WHERE name = @name";
            var parameters = new DynamicParameters();
            parameters.Add("name", dbName);


            using var connection = await _connectionFactory.CreateMasterConnectionAsync();
            var records = connection.Query(query, parameters);
            if (!records.Any())
            {
                await connection.ExecuteAsync($"CREATE DATABASE [{dbName}]");
            }


        }
    }
}

/*
 
                 await connection.ExecuteAsync($@"USE [{dbName}]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[PoliticalParties] (

    [Id][int] IDENTITY(1, 1) NOT NULL,

    [FrontEndId] [uniqueidentifier] NOT NULL,

    [Name] [nvarchar] (max)NOT NULL,
	[ImageUrl][nvarchar] (max)NOT NULL,
 CONSTRAINT[PK_PoliticalParties] PRIMARY KEY CLUSTERED 
(

    [Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Politicians] (

    [Id][int] IDENTITY(1, 1) NOT NULL,

    [FrontEndId] [uniqueidentifier] NOT NULL,

    [FullName] [nvarchar] (max)NOT NULL,
	[BirthDate][datetime2] (7) NOT NULL,

    [InstagramUrl] [nvarchar] (max)NULL,
	[TwitterUrl][nvarchar] (max)NULL,
	[FacebookUrl][nvarchar] (max)NULL,
	[PoliticalPartyId][int] NOT NULL,
 CONSTRAINT[PK_Politicians] PRIMARY KEY CLUSTERED 
(

    [Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]

SET IDENTITY_INSERT[dbo].[PoliticalParties] ON

INSERT[dbo].[PoliticalParties] ([Id], [FrontEndId], [Name], [ImageUrl]) VALUES(1, N'08e8fb6e-b166-4ec4-ae4e-6b3096f84e1b', N'SPD', N'')

INSERT[dbo].[PoliticalParties]([Id], [FrontEndId], [Name], [ImageUrl]) VALUES(2, N'24ed715a-31cb-4fca-9726-533072b0c79d', N'ANO', N'')

SET IDENTITY_INSERT[dbo].[PoliticalParties] OFF
SET IDENTITY_INSERT [dbo].[Politicians] ON

INSERT[dbo].[Politicians] ([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(1, N'a5e15559-ebba-426a-8a38-f56e3421903c', N'Tomio', CAST(N'1966-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians]([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(2, N'734dfb56-5870-4487-aae2-12b207f0bcd2', N'TOnda', CAST(N'1988-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians]([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(3, N'8cf61bec-bd1d-4227-aa9e-76338e2fea2c', N'Karel', CAST(N'1996-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians]([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(4, N'996b175f-2cff-40a5-b568-0fbc74328a66', N'Pavel', CAST(N'2000-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 1)

INSERT[dbo].[Politicians]([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(5, N'4756c20d-826f-4c7c-89d0-3252e130546d', N'Andrej', CAST(N'1962-05-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 2)

INSERT[dbo].[Politicians]([Id], [FrontEndId], [FullName], [BirthDate], [InstagramUrl], [TwitterUrl], [FacebookUrl], [PoliticalPartyId]) VALUES(6, N'65e86328-5f07-45b6-b5fe-4e6be3d98d2f', N'Karel', CAST(N'1977-04-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 2)

SET IDENTITY_INSERT[dbo].[Politicians] OFF

CREATE NONCLUSTERED INDEX [IX_PoliticalParties_FrontEndId] ON[dbo].[PoliticalParties]
(

    [FrontEndId] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Politicians_FrontEndId] ON[dbo].[Politicians]
(

    [FrontEndId] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Politicians_PoliticalPartyId] ON[dbo].[Politicians]
(

    [PoliticalPartyId] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]

ALTER TABLE[dbo].[Politicians] WITH CHECK ADD  CONSTRAINT [FK_Politicians_PoliticalParties_PoliticalPartyId] FOREIGN KEY([PoliticalPartyId])
REFERENCES[dbo].[PoliticalParties]([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[Politicians] CHECK CONSTRAINT[FK_Politicians_PoliticalParties_PoliticalPartyId]

");
 
 */
