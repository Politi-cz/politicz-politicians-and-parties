using System.Text.Json;

namespace politicz_politicians_and_parties.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        
        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}
