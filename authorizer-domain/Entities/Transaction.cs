using Newtonsoft.Json;
using System;

namespace authorizer_domain.Entities
{
    public class Transaction
    {
        [JsonProperty("merchant")]
        public string Merchant { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }

    public class TransactionData
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}