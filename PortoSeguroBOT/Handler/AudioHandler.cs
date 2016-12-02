using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PortoSeguroBOT.Handler
{
    public class AudioHandler
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string DoSpeechReco(Attachment attachment)
        {
            
            string headerValue;
            // Note: Sign up at https://microsoft.com/cognitive to get a subscription key.  
            // Use the subscription key as Client secret below.
            string requestUri = "https://speech.platform.bing.com/recognize";

            //URI Params. Refer to the Speech API documentation for more information.
            requestUri += @"?scenarios=ulm";                                // websearch is the other main option.
            requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";   // You must use this ID.
            requestUri += @"&locale=pt-BR";                                 // read docs, for other supported languages. 
            requestUri += @"&device.os=wp7";
            requestUri += @"&version=3.0";
            requestUri += @"&format=json";
            requestUri += @"&instanceid=" + Guid.NewGuid().ToString();
            requestUri += @"&requestid=" + Guid.NewGuid().ToString();

            string host = @"speech.platform.bing.com";
            //string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
            string contentType = @"audio/wav; samplerate=16000";
            string AudioURL = "https://cdn.fbsbx.com/v/t59.3654-21/15203807_1888413331386802_277733295691661312_n.mp4/audioclip-1480623094000-3584.mp4?oh=3cfb807539eeb3d8c6cb551d81dc1244&oe=58433821";
            //var wav = HttpWebRequest.Create(attachment.ContentUrl);
            var wav = HttpWebRequest.Create(AudioURL);
            string responseString = string.Empty;

            try
            {
                string token = GetSpeechToken(Constants.SpeechSecretKey);
                Console.WriteLine("Token: {0}\n", token);

                //Create a header with the access_token property of the returned token
                headerValue = "Bearer " + token;
                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);

                HttpWebRequest request = null;
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;
                request.Host = host;
                request.ContentType = contentType;
                request.Headers["Authorization"] = headerValue;
                request.Timeout = 20000;

                using (Stream wavStream = wav.GetResponse().GetResponseStream())
                {
                    byte[] buffer = null;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        int count = 0;
                        do
                        {
                            buffer = new byte[1024];
                            count = wavStream.Read(buffer, 0, 1024);
                            requestStream.Write(buffer, 0, count);
                        } while (wavStream.CanRead && count > 0);
                        // Flush
                        requestStream.Flush();
                    }
                    //Get the response from the service.
                    Console.WriteLine("Response:");
                    using (WebResponse response = request.GetResponse())
                    {
                        Console.WriteLine(((HttpWebResponse)response).StatusCode);
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseString = sr.ReadToEnd();
                        }
                        Console.WriteLine(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info("UsuárioVoz [ERROR]: " + ex.Message);
            }
            dynamic data = JObject.Parse(responseString);
            return data.header.name;
        }

        public static string GetSpeechToken(string secretKey)
        {
            string URL = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
         
            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Ocp-Apim-Subscription-Key", secretKey);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentLength = 0;

            string token = "";
            try
            {
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    token = reader.ReadToEnd();
                  
                }
            }
            catch (Exception e)
            {

            }

            return token;
        }
        
    }
}