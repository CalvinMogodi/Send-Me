using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Models
{
    public class Request
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("tolocation")]
        public Location Tolocation { get; set; }

        [JsonProperty("fromLocation")]
        public Location FromLocation { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("packageSize")]
        public string PackageSize { get; set; }

        [JsonProperty("vehicleBodyType")]
        public string VehicleBodyType { get; set; }
    }
}
