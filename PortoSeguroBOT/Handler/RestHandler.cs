using Newtonsoft.Json;
using PortoSeguroBOT.ChatInterface.DirectLineAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace PortoSeguroBOT.Handler
{
    public class RESTHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {

            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;

            string RawUrl = Request.RawUrl;
            string splitter = "/?";
            string SubRawUrl = RawUrl.Substring(RawUrl.IndexOf(splitter) + splitter.Length);
            string[] Parameters = SubRawUrl.Split('/');
            JavaScriptSerializer jc = new JavaScriptSerializer();
            DirectLineApi apiData = new DirectLineApi();
            
            if (Request.RequestType == "POST")
            {
                if (Request.ContentType.Contains("application/json"))
                {
                    if ("getToken".Equals(Parameters[1]))
                    {
                       
                        try
                        {
                            dynamic tokenData = apiData.getDirectLineToken();
                            apiData.ConversationId = tokenData.conversationId;
                            apiData.ConversationToken = tokenData.token;
                        }
                        catch (Exception e)
                        {

                        }
                        Response.Write(jc.Serialize(apiData));
                        Response.ContentType = "application/json";
                    }
                    else if ("botToUser".Equals(Parameters[1]))
                    {
                        try
                        {
                            Conversation conv = apiData.getConversation(Parameters[2],Parameters[3]);
                            Response.Write(jc.Serialize(conv));
                            Response.ContentType = "application/json";
                        }
                        catch (Exception e)
                        {
                            Response.Write("");
                            Response.ContentType = "application/json";
                        }
                    }
                    else if ("userToBot".Equals(Parameters[1]))
                    {
                        try
                        {
                            string msg;
                            using (var reader = new StreamReader(Request.InputStream))
                            {
                                dynamic objJson = JsonConvert.DeserializeObject(reader.ReadToEnd());
                                msg = objJson.text.Value;
                            }
                            apiData.sendMsgToBot(Parameters[2], msg);
                            Response.Write(jc.Serialize("{success:true}"));
                            Response.ContentType = "application/json";
                        }
                        catch (Exception e)
                        {
                            Response.Write(jc.Serialize("{success:false}"));
                            Response.ContentType = "application/json";
                        }
                    }
                }
            }
         
        }

        #endregion

     
    }
}