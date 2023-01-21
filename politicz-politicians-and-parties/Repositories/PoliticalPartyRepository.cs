﻿using Dapper;
using FluentMigrator.Model;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Repositories
{
    public class PoliticalPartyRepository : IPoliticalPartyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PoliticalPartyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<PoliticalParty>> GetPoliticalPartiesAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            return await connection.QueryAsync<PoliticalParty>("SELECT * FROM PoliticalParties");
        }

        public async Task<PoliticalParty?> GetPoliticalPartyAsync(Guid frontEndId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = @"SELECT pp.Id, pp.FrontEndId, [Name], [ImageUrl], p.Id, p.FrontEndId, BirthDate, FullName, InstagramUrl, TwitterUrl, FacebookUrl, PoliticalPartyId
                        FROM PoliticalParties pp LEFT JOIN Politicians p ON pp.Id = p.PoliticalPartyId
                        WHERE pp.FrontEndId = @FrontEndId ";

            // I know that where will be at most 1 political party thanks to WHERE clause so I can collect politicians on the way and skip the grouping part of result that gets rid of duplicated
            var politicians = new List<Politician>();
            var politicalParties = await connection.QueryAsync<PoliticalParty, Politician, PoliticalParty>(sql, (politicalParty, politician) =>
            {
                if (politician is not null) { 
                    politicians.Add(politician);
                }

                return politicalParty;
            },param: new { FrontEndId = frontEndId }, splitOn: "Id");

            if (politicalParties.Any()) {
                var politicalParty = politicalParties.First();
                politicalParty.Politicians = politicians;

                return politicalParty;
            }

            return null;

        }
    }
}
