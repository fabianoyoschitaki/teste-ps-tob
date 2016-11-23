using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.ChatInterface.DirectLineAPI
{
    public class Conversation
    {
        public IList<dynamic> Messages { get; set; }
        public string WaterMark { get; set; }
    }
}