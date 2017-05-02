using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WithoutPath.EVEAPI.JsonTypes
{
    public class ShipResponse
    {
        [JsonProperty("ship_item_id")]
        public long ship_item_id { get; set; }
        [JsonProperty("ship_name")]
        public string ship_name { get; set; }
        [JsonProperty("ship_type_id")]
        public long ship_type_id { get; set; }
    }
}
