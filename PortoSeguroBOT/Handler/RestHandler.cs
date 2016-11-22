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

            if (Request.RequestType == "GET")
            {

                //RawUrl=http://localhost/Services.fck/?Vivek/Gupta
                string RawUrl = Request.RawUrl;
                string splitter = "/?";
                string SubRawUrl = RawUrl.Substring(RawUrl.IndexOf(splitter) + splitter.Length);

                string[] Parameters = SubRawUrl.Split('/');
                if (Parameters.Length >= 2)
                {
                    string name = Parameters[0];
                    string surname = Parameters[1];
                    string res = string.Format("Welcome {0} {1}", name, surname);
                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    StringBuilder sb = new StringBuilder();
                    jc.Serialize(res, sb);
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
            }
            else if (Request.RequestType == "POST")
            {

                if (Request.ContentType.Contains("text/xml"))
                {
                    Request.InputStream.Seek(0, SeekOrigin.Begin);
                    XmlDocument xm = new XmlDocument();
                    xm.Load(Request.InputStream);

                    output.name = xm.DocumentElement["name"].InnerText;
                    output.surname = xm.DocumentElement["surname"].InnerText;

                    XmlSerializer xr = new XmlSerializer(typeof(output));
                    MemoryStream mr = new MemoryStream();
                    xr.Serialize(mr, new output());
                    byte[] OutXmlByte = mr.ToArray();

                    Response.OutputStream.Write(OutXmlByte, 0, OutXmlByte.Length);
                    Response.ContentType = "text/xml";
                }
                else if (Request.ContentType.Contains("application/json"))
                {
                    string data = Encoding.UTF8.GetString(Request.BinaryRead(Request.TotalBytes));
                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    Dictionary<string, string> keyValue = jc.Deserialize<Dictionary<string, string>>(data);

                    output.name = keyValue["name"];
                    output.surname = keyValue["surname"];

                    Response.Write(jc.Serialize(new output()));
                    Response.ContentType = "application/json";
                }
                else if (Request.ContentType.Contains("text/plain"))
                {
                    string data = Encoding.UTF8.GetString(Request.BinaryRead(Request.TotalBytes));
                    string[] keyValue = data.Split(',');

                    output.name = keyValue[0];
                    output.surname = keyValue[1];

                    Response.Write(new output().result);
                    Response.ContentType = "text/plain";
                }
            }
        }

        #endregion

        public class output
        {
            public output()
            {
                result = "Welcome " + name + " " + surname;
            }

            public string result;

            [NonSerialized]
            public static string name;
            [NonSerialized]
            public static string surname;

        }
    }
}