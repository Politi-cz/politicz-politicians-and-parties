namespace PoliticiansAndParties.Api.Database;

public readonly struct ConnectionStrings
{
    public ConnectionStrings(string masterConnection, string sqlConnection)
    {
        MasterConnection = masterConnection;
        SqlConnection = sqlConnection;
    }

    public string MasterConnection { get; }

    public string SqlConnection { get; }
}
