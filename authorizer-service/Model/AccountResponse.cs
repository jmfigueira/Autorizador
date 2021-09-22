using Newtonsoft.Json;
using System.Collections.Generic;

namespace authorizer_service.Model
{
    public class Account
    {
        [JsonProperty("active-card")]
        public bool ActiveCard { get; set; }

        [JsonProperty("available-limit")]
        public int AvailableLimit { get; set; }
    }

    public class AccountResponse
    {
        [JsonProperty("account")]
        public Account Account { get; set; }

        [JsonProperty("violations")]
        public List<string> Violations { get; set; }
    }
}
