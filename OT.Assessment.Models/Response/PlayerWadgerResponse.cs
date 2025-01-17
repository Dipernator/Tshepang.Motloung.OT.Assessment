﻿using System.Text.Json.Serialization;

namespace OT.Assessment.Models.Response
{
    public class PlayerWadgerResponse
    {
        [JsonPropertyName("playerWadgers")]
        public List<PlayerWadger> PlayerWadgers { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
}
