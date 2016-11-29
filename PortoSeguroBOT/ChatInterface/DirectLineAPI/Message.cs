using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.ChatInterface.DirectLineAPI
{
    public class Message
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public string From { get; set; }
        public string Text { get; set; }  
        public string Raw { get; set; }
    }
}