using Newtonsoft.Json;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace PortoSeguroBOT.ChatInterface.DirectLineAPI
{
    public class DirectLineApi
    {
        public string ConversationId { get; set; }
        public string ConversationToken { get; set; }
        public int WaterMark { get; set; }

        public DirectLineApi()
        {
            this.WaterMark = 0;
        }

        public dynamic getDirectLineToken()
        {
            // Create the web request  
            string URL = "https://directline.botframework.com/api/conversations"; 
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = 0;
            request.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + Constants.DirectLineApi);

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
                    return objJson;
                }
            }
            catch (Exception e)
            {
                return objJson;
            }
           
        }

        public Conversation getConversation(string ConversationID, string WaterMark)
        {
            // Create the web request  
            string URL = "https://directline.botframework.com/api/conversations";
            URL += "/";
            URL += ConversationID;
            URL += "/messages?watermark=";
            URL += WaterMark;

            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            
            request.Method = "GET";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = 0;
            request.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + Constants.DirectLineApi);

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
                    Conversation conv = new Conversation();
                    IList<object> listMsg = new List<object>();
                    foreach (var msg in objJson.messages)
                    {
                        Message nmsg = new Message();
                        nmsg.Id = msg.id.Value;
                        nmsg.ConversationId = msg.conversationId.Value;
                        nmsg.Text = msg.text.Value;
                        nmsg.From = msg.from.Value;
                        listMsg.Add(nmsg);
                    }
                    conv.Messages = listMsg;
                    conv.WaterMark = objJson.watermark;
                    return conv;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public Conversation sendMsgToBot(string ConversationID, string msg)
        {
            // Create the web request  
            string URL = "https://directline.botframework.com/api/conversations";
            URL += "/";
            URL += ConversationID;
            URL += "/messages";

            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            string jsonMsg = "{text:" + msg + ", from:" + ConversationID + "}";

            char[] buffer = jsonMsg.ToCharArray();
            request.Method = "POST";
            request.ContentLength = buffer.Length;
            request.ContentType = "application/json; charset=utf-8";
            
            request.ContentLength = 0;
            request.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + Constants.DirectLineApi);

            Stream newStream = request.GetRequestStream();
            //newStream.Write(buffer, 0, buffer.Length);
            newStream.Close();

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
                   
                    return conv;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}