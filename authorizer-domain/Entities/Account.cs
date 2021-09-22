using Newtonsoft.Json;

namespace authorizer_domain.Entities
{
    public class Account
    {
        [JsonProperty("active-card")]
        public bool ActiveCard { get; set; }

        [JsonProperty("available-limit")]
        public int AvailableLimit { get; set; }
    }

    public class AccountData
    {
        [JsonProperty("account")]
        public Account Account { get; set; }
    }
}