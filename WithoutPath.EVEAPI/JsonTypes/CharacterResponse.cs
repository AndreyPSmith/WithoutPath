using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WithoutPath.EVEAPI.JsonTypes
{
    public  class CharacterResponse
    {
        [JsonProperty("corporation_id")]
        public long corporation_id { get; set; }
        [JsonProperty("birthday")]
        public string birthday { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("gender")]
        public string gender { get; set; }
        [JsonProperty("race_id")]
        public int race_id { get; set; }
        [JsonProperty("bloodline_id")]
        public int bloodline_id { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("ancestry_id")]
        public int ancestry_id { get; set; }
        [JsonProperty("security_status")]
        public object security_status { get; set; }
    }
}
