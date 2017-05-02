using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WithoutPath.EVEAPI.JsonTypes
{
    public class LocationResponse
    {
        [JsonProperty("solar_system_id")]
        public long solar_system_id { get; set; }
        [JsonProperty("structure_id")]
        public long? structure_id { get; set; }
    }
}