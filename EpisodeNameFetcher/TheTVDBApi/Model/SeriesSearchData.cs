using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeNameFetcher.TheTVDBApi.Model
{
    class SeriesSearchData
    {
        [JsonProperty("aliases")]
        public string[] Aliases { get; set; }
        [JsonProperty("banner")]
        public string Banner { get; set; }
        [JsonProperty("firstAired")]
        public string FirstAired { get; set; }
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("network")]
        public string Network { get; set; }
        [JsonProperty("overview")]
        public string Overview { get; set; }
        [JsonProperty("seriesName")]
        public string SeriesName { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
