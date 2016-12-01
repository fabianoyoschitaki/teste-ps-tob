using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace PortoSeguroBOT.Bean
{
    public class PortoSearch
    {
        public static List<SearchResult> GetPortoSearch(string text)
        {
            StringBuilder URL = new StringBuilder();
            URL.Append("https://brazil.inbenta.com/portoseguro?q=");
            URL.Append(text);

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;

            List<SearchResult> SearchList = new List<SearchResult>();
            try
            {
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string data = reader.ReadToEnd();
                    data = data.Substring(data.IndexOf("<div id=\"results\" class=\"faqResults\">"));
                    data = data.Substring(0, data.IndexOf("<div id=\"crossSelling\" class=\"csContainer column4\">"));
                    data = "<div><div>" + data;
                    data = data.Replace("href", " href").Replace("&nbsp;", "").Replace("checked", "");

                    var xDoc = XDocument.Parse(data);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xDoc.ToString());
                    string JsonResult = JsonConvert.SerializeXmlNode(doc);
                    dynamic objJson = JsonConvert.DeserializeObject(JsonResult);

                    for(var d = 0; d < objJson.div.div.div.div.Count; d++)
                    {
                        if(d%2 == 0)
                        {
                            var d2 = d + 1;
                            try { 
                                SearchResult s = new SearchResult();
                                s.Title = objJson.div.div.div.div[d].a["#text"].Value;
                                string Desc = "";
                                if (objJson.div.div.div.div[d2].div[0].div["#text"] is JArray)
                                {
                                    foreach (dynamic txt in objJson.div.div.div.div[d2].div[0].div["#text"])
                                    {
                                        Desc += txt.Value;
                                    }
                                }
                                else
                                {
                                    Desc = objJson.div.div.div.div[d2].div[0].div["#text"].Value;
                                }

                                s.Desc = Desc;
                                s.SubDesc = "Resultado da Porto Seguro";
                                s.Link = objJson.div.div.div.div[d2].div[0].div.a["@href"].Value;
                                SearchList.Add(s);
                            } catch (Exception e)
                            {

                            }
                            d = d2;
                        }
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