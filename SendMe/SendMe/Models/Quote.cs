using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Models
{
    public class Quote
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("courierName")]
        public string CourierName { get; set; }

        [JsonProperty("courierMobileNumber")]
        public string CourierMobileNumber { get; set; }

        [JsonProperty("courierKmDistance")]
        public decimal CourierKmDistance { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("courierProfilePicture")]
        public string CourierProfilePicture { get; set; }
    }
}
