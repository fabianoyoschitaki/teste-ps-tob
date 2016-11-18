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
        //log4net
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string tex;
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                
                // mensagem para retornar
                string messageToReply = "Benvindo a Porto Seguro, como podemos te ajudar? ";
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                Usuario user = userData.GetProperty<Usuario>("UserData");
                // se o usuário existir, recupera
                if (user != null)
                {
                    string UserFirstName = Formatters.Capitalize(user.Nome).Split( )[0];
                    messageToReply = $"Benvindo a Porto Seguro {UserFirstName}, como podemos te ajudar? ";                    
                }
                log.Info(messageToReply);
                await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(messageToReply));
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new RootLuisDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
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