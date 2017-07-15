using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Models
{
    public class Courier
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("courierId")]
        public string CourierId { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("pricePerKM")]
        public double PricePerKM { get; set; }

        [JsonProperty("extraCharges")]
        public double ExtraCharges { get; set; }

        [JsonProperty("vehicleBodyTypes")]
        public List<string> VehicleBodyTypes { get; set; }
    }
}
