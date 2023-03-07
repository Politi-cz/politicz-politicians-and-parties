namespace PoliticiansAndParties.Api.Models;

public class ErrorDetail
{
    public ErrorDetail(string message, IDictionary<string, string[]>? errors = null)
    {
        Message = message;
        Errors = errors;
    }

    public string Message { get; set; }

    public IDictionary<string, string[]>? Errors { get; set; }
}
