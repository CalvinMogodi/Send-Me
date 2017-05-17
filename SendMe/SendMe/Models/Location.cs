using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Models
{
    public class Location
    {
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("placeId")]
        public string PlaceId { get; set; }
    }
}
