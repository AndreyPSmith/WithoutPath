using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WithoutPath.EVEAPI.JsonTypes
{
    public class ShipTypeResponse
    {
        [JsonProperty("type_id")]
        public long type_id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }      
    }
}
