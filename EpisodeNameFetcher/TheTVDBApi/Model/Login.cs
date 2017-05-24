using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeNameFetcher.TheTVDBApi.Model
{
    class Login
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; set; }
        [JsonProperty("userkey")]
        public string UserKey { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
    }
}
