using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeNameFetcher.TheTVDBApi.Model
{
    class BasicEpisode
    {
        [JsonProperty("absoluteNumber")]
        public int AbsoluteNumber { get; set; }
        [JsonProperty("airedEpisodeNumber")]
        public int AiredEpisodeNumber { get; set; }
        [JsonProperty("airedSeason")]
        public int AiredSeason { get; set; }
        [JsonProperty("dvdEpisodeNumber")]
        public decimal DvdEpisodeNumber { get; set; }
        [JsonProperty("dvdSeason")]
        public int DvdSeason { get; set; }
        [JsonProperty("episodeName")]
        public string EpisodeName { get; set; }
        [JsonProperty("firstAired")]
        public DateTime FirstAired { get; set; }
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("lastUpdated")]
        public int LastUpdated { get; set; }
        [JsonProperty("overview")]
        public string Overview { get; set; }
    }
}
