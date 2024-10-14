using System.Text.Json.Serialization;

namespace OT.Assessment.Models.Response
{
    public class TopSpenderResponse
    {
        [JsonPropertyName("accountId")]
        public Guid AccountId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("totalAmountSpend")]
        public double TotalAmountSpend { get; set; }
    }
}
