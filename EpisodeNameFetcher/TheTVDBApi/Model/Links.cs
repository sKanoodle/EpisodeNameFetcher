using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeNameFetcher.TheTVDBApi.Model
{
    class Links
    {
        [JsonProperty("first")]
        public int First { get; set; }
        [JsonProperty("last")]
        public int Last { get; set; }
        [JsonProperty("next")]
        public int Next { get; set; }
        [JsonProperty("previous")]
        public int Previous { get; set; }
    }
}
