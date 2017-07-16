using Newtonsoft.Json;
using SendMe.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Models
{
    public class QuoteRequest
    {
        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("osVersion")]
        public string OSVersion { get; set; }

        [JsonProperty("requestDatetime")]
        public DateTime RequestDatetime { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("quotes")]
        public ObservableRangeCollection<Quote> Quotes { get; set; }
    }
}
