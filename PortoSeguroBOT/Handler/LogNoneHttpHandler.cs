using System;
using System.IO;
using System.Web;

namespace PortoSeguroBOT.Handler
{
    public class LogNoneHttpHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string logStr = "NOT_FOUND";
            try
            {
                logStr = File.ReadAllText(GetBasePath() + "/bot_none.log");
            }
            catch (Exception e)
            {
                logStr = e.Message;
            }

            HttpResponse response = context.Response;
            response.Write("<html><body><pre>");
            response.Write(logStr);
            response.Write("</pre></body></html>");
        }

        public static string GetBasePath()
        {
            if (System.Web.HttpContext.Current == null) return AppDomain.CurrentDomain.BaseDirectory;
            else return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "");
        }
        #endregion
    }
}
