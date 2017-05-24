using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeNameFetcher.TheTVDBApi.Model
{
    class SeriesEpisodes
    {
        [JsonProperty("data")]
        public BasicEpisode[] Data { get; set; }
        [JsonProperty("errors")]
        public JsonErrors Errors { get; set; }
        [JsonProperty("links")]
        public Links Links { get; set; }
    }
}
