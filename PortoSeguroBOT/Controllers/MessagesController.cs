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
using PortoSeguroBOT.Bean;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.ServiceModel.Description;
using PortoSeguroBOT.Helpers;

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

                Usuario user = new Usuario();
                Activity reply = null;
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                user = userData.GetProperty<Usuario>("UserData");
                if (user != null)
                {
                    string UserFirstName = Formatters.Capitalize(user.Nome).Split( )[0];
                    reply = activity.CreateReply($"Benvindo a Porto Seguro {UserFirstName}, como podemos te ajudar? ");
                }
                else
                {
                    reply = activity.CreateReply("Benvindo a Porto Seguro, como podemos te ajudar? ");
                }
                
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