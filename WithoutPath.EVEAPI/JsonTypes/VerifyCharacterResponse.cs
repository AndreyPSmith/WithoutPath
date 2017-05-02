using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WithoutPath.EVEAPI.JsonTypes
{
    public class VerifyCharacterResponse
    {
        [JsonProperty("CharacterID")]
        public long CharacterID { get; set; }

        [JsonProperty("CharacterName")]
        public string CharacterName { get; set; }

        [JsonProperty("ExpiresOn")]
        public string ExpiresOn { get; set; }

        [JsonProperty("Scopes")]
        public string Scopes { get; set; }

        [JsonProperty("TokenType")]
        public string TokenType { get; set; }

        [JsonProperty("CharacterOwnerHash")]
        public string CharacterOwnerHash { get; set; }
    }
}
