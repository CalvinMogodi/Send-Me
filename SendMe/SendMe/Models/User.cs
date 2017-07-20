using Newtonsoft.Json;
using SendMe.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Models
{
    public class User
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("profilePicture")]
        public string ProfilePicture { get; set; }

        [JsonProperty("userTypeId")]
        public int UserTypeId { get; set; }

        [JsonProperty("oneTimePin")]
        public int OneTimePin { get; set; }

        [JsonProperty("oneTimePinDateTime")]
        public DateTime OneTimePinDateTime { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("currentLocation")]
        public Location CurrentLocation {get; set;}

        [JsonProperty("userType")]
        public UserType UserType { get; set; }

        [JsonProperty("courier")]
        public Courier Courier { get; set; }
    }
}
