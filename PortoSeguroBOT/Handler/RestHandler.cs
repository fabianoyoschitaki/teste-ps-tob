using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
           
            HttpResponse response = context.Response;
            response.Write("<html><body><pre>");
            response.Write("<h1>OI!</h1>");
            response.Write("</pre></body></html>");
        }

        #endregion
    }
}