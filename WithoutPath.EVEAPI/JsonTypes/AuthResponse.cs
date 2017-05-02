using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace WithoutPath.EVEAPI.JsonTypes
{
    public class AuthResponse
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }

        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("expires_in")]
        public int expires_in { get; set; }

        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }
    }
}
