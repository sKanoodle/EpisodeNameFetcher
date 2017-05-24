using EpisodeNameFetcher.TheTVDBApi.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EpisodeNameFetcher.TheTVDBApi
{
    class TheTVDBApiClient
    {
        public class Parameter
        {
            public string Title { get; set; }
            public string Value { get; set; }

            public string EncodedValue => HttpUtility.UrlEncode(Value);

            public Parameter(string title, object value)
                :this(title, value.ToString()) { }

            public Parameter(string title, string value)
            {
                Title = title ?? throw new ArgumentNullException(nameof(title));
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private string Token;
        public string BaseUrl { get; set; }

        private string BuildUrl(params string[] parts)
        {
            if (parts?.Length < 1)
                throw new ArgumentNullException(nameof(parts));

            if (parts.Length == 1)
                return parts[0];

            StringBuilder builder = new StringBuilder(parts[0]);
            for (int i = 1; i < parts.Length; i++)
            {
                if (builder[builder.Length - 1] != '/' && !parts[i].StartsWith("/"))
                    builder.Append("/");
                builder.Append(parts[i]);
            }
            return builder.ToString();
        }

        private string PerformRequest(string endpoint, params Parameter[] parameters)
        {
            return PerformRequest(endpoint, null, parameters);
        }

        private string PerformRequest(string endpoint, string body, params Parameter[] parameters)
        {
            StringBuilder parameterString = new StringBuilder(endpoint);
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i == 0)
                    parameterString.Append("?");
                else
                    parameterString.Append("&");
                parameterString.AppendFormat("{0}={1}", parameters[i].Title, parameters[i].EncodedValue);
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BuildUrl(BaseUrl, parameterString.ToString()));
            request.Accept = "application/json";
            request.Headers.Add("Accept-Language", "en");
            request.Headers.Add("Authorization", $"Bearer {Token}");

            if (body != null)
            {
                request.ContentType = "application/json";
                request.Method = "POST";

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(body);
                    writer.Flush();
                    writer.Close();
                }
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        private T GetResponse<T>(string endpoint, params Parameter[] parameters)
        {
            return GetResponse<T>(endpoint, null, parameters);
        }

        private T GetResponse<T>(string endpoint, string body, params Parameter[] parameters)
        {
            string result = PerformRequest(endpoint, body, parameters);
            return JsonConvert.DeserializeObject<T>(result, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore });
        }

        private string GetToken(string apiKey)
        {
            string body = JsonConvert.SerializeObject(new Login { ApiKey = apiKey });
            return GetResponse<TokenResponse>("login", body).Token;
        }

        public SeriesSearchData[] SearchShow(string searchQuery)
        {
            return GetResponse<DataWrapper<SeriesSearchData[]>>("search/series", new Parameter("name", searchQuery)).Data;
        }

        public BasicEpisode[] Episodes(int showID)
        {
            List<BasicEpisode> result = new List<BasicEpisode>();
            int page = 1;
            SeriesEpisodes eps;
            do
                result.AddRange((eps = GetResponse<SeriesEpisodes>($"series/{showID}/episodes", new Parameter("page", page++))).Data);
            while (eps.Links.Next != 0);
            return result.ToArray();
        }

        public TheTVDBApiClient(string baseUrl, string apiKey)
        {
            BaseUrl = baseUrl;
            Token = GetToken(apiKey);
        }
    }
}
