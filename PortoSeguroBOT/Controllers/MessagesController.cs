using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using PortoSeguroBOT.Dialogs;

namespace PortoSeguroBOT
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        public static string tex;
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
               // Activity reply = activity.CreateReply("[MessagesController] Benvindo a Porto Seguro, como podemos te ajudar?");
                Activity reply = activity.CreateReply("Benvindo a Porto Seguro, como podemos te ajudar? ");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                    await Conversation.SendAsync(activity, () => new RootLuisDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}