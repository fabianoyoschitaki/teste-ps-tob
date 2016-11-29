using Newtonsoft.Json;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            string URL = "https://directline.botframework.com/v3/directline/conversations"; 
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

        public string getConversation(string ConversationID, string WaterMark)
        {
            // Create the web request  
            string URL = "https://directline.botframework.com/v3/directline/conversations";
            URL += "/";
            URL += ConversationID;
            URL += "/activities?watermark=";
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
                    //objJson = JsonConvert.DeserializeObject(JSON);
                    //Conversation conv = new Conversation();
                    //IList<object> listMsg = new List<object>();
                    //foreach (var msg in objJson.activities)
                    //{
                    //    Message nmsg = new Message();
                    //    nmsg.Id = msg.id.Value;
                    //    nmsg.ConversationId = msg.conversation.id.Value;
                    //    nmsg.Text = msg.text.Value;
                    //    nmsg.From = msg.from.id.Value;
                    //    nmsg.Raw = JSON;
                    //    listMsg.Add(nmsg);
                    //}
                    //conv.Messages = listMsg;
                    //conv.WaterMark = objJson.watermark;
                    return JSON;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public void sendMsgToBot(string ConversationID, string msg)
        {
            // Create the web request  
            string URL = "https://directline.botframework.com/v3/directline/conversations";
            URL += "/";
            URL += ConversationID;
            URL += "/activities";

            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            string jsonMsg = "{type:\"message\", text:\"" + msg + "\", from: { id:\"" + ConversationID + "\"}}";

            byte[] buffer = Encoding.ASCII.GetBytes(jsonMsg);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            
            request.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + Constants.DirectLineApi);

            Stream newStream = request.GetRequestStream();
            newStream.Write(buffer, 0, buffer.Length);
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
                }
            }
            catch (Exception e)
            {
                string err = e.ToString();
            }

        }
    }
}