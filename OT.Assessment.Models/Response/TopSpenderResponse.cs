using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
