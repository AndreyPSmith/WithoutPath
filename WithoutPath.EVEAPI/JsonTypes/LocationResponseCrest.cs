using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WithoutPath.EVEAPI.JsonTypes
{
    public class LocationResponseCrest
    {
        [JsonProperty("solarSystem")]
        public LocationResponseSolarSystem solar_system { get; set; }
    }

    public class LocationResponseSolarSystem
    {
        [JsonProperty("id_str")]
        public string solar_system { get; set; }

        [JsonProperty("id")]
        public long solar_system_id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

    }
}
