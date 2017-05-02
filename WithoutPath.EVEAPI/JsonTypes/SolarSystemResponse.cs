using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WithoutPath.EVEAPI.JsonTypes
{
    public class SolarSystemResponse
    {
        [JsonProperty("constellation_id")]
        public int constellation_id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("security_class")]
        public string security_class { get; set; }
        [JsonProperty("security_status")]
        public double? security_status { get; set; }
        [JsonProperty("stargates")]
        public List<int> stargates { get; set; }
        [JsonProperty("system_id")]
        public int system_id { get; set; }
    }
}
