using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeNameFetcher.TheTVDBApi.Model
{
    class DataWrapper<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
