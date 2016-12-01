using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PortoSeguroBOT.Bean
{
    public class MicrosftAPI
    {
        public static List<SearchResult> BingSearch(string text)
        {
            StringBuilder URL = new StringBuilder();
            URL.Append("https://api.cognitive.microsoft.com/bing/v5.0/search?count=3&mkt=pt-BR&safesearch=Moderate&q=");
            URL.Append(text);

            List<SearchResult> SearchList = new List<SearchResult>();
            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Ocp-Apim-Subscription-Key", "0fecd33263264d4387476e2f1d325982");

            dynamic objJson = null;
            try
            {
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string JSON = reader.ReadToEnd();
                    objJson = JsonConvert.DeserializeObject(JSON);
                    foreach (dynamic pg in objJson.webPages.value)
                    {
                        SearchResult s = new SearchResult();
                        s.Title = pg.name.Value;
                        s.Desc = pg.snippet.Value;
                        s.SubDesc = "Resultado do Bing";
                        s.Link = pg.url.Value;
                        SearchList.Add(s);
                    }

                }
            }
            catch (Exception e)
            {

            }

            return SearchList;
        }
    }
}