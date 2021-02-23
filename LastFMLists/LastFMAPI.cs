using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace LastFMLists
{
    class LastFMAPI
    {
        private const string apiKey = "тут апи ключ";
        public List<Track> GetTracks(int page)
        {
            List<Track> tracksList = new List<Track>();
            string URL = string.Format("http://ws.audioscrobbler.com/2.0/?method=chart.gettoptracks&api_key={0}&format=json&page={1}&limit=50", apiKey, page);
            dynamic res = JsonConvert.DeserializeObject(this.request(URL, "GET"));
            var tracks = res.tracks.track;
            foreach (var track in tracks)
            {
                Track track_ = new Track
                {
                    name = track.name,
                    link = track.url,
                    artist = track.artist.name,
                    url = track.url,
                    image = track.image[0]["#text"],
                    tags = this.getTopTags((string)track.name, (string)track.artist.name)
                };
                tracksList.Add(track_);
            }
            return tracksList;

        }
        private List<string> getTopTags(string name, string artist)
        {
            List<string> tags = new List<string>();
            string URL = string.Format("http://ws.audioscrobbler.com/2.0/?method=track.gettoptags&artist={0}&track={1}&api_key={2}&format=json", artist, name, apiKey); ;
            dynamic json = JsonConvert.DeserializeObject(request(URL, "GET"));
            try
            {
                var tagsJSON = json.toptags.tag;
                foreach (var tag in tagsJSON)
                {
                    tags.Add((string)tag.name);
                }
            }
            catch (Exception err) { }
            return tags;
        }
        private string request(string url, string method)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = method;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
    }
}
