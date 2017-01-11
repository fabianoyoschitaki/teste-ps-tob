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
using PortoSeguroBOT.Handler;

namespace PortoSeguroBOT
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string tex;

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                // mensagem para retornar
                string messageToReply = "Benvindo a Porto Seguro, como podemos te ajudar?";
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                Usuario user = userData.GetProperty<Usuario>("UserData");
                // se o usuário existir, recupera
                if (user != null)
                {
                    string UserFirstName = Formatters.Capitalize(user.Nome).Split( )[0];
                    messageToReply = $"Benvindo a Porto Seguro {UserFirstName}, como podemos te ajudar?";                    
                }
                log.Info("[BOT]["+activity.ChannelId + "][" + activity.From.Id + "]: " + messageToReply);
                await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(messageToReply));
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                try
                {
                    if (activity.Attachments.Any())
                    {
                        foreach(Attachment at in activity.Attachments)
                        {
                            try
                            {
                                log.Info("UsuárioAtt [" + activity.From.Id + "] Name:" + at.Name + " ThumbnailUrl:" + at.ThumbnailUrl + " ContentUrl:" + at.ContentUrl + " ContentType:" + at.ContentType);
                                await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("Desculpe, no momento eu não consigo entender emoticons e anexos, por favor, use apenas texto."));
                            }
                            catch (Exception e)
                            {
                                log.Error("Erro: " + e.ToString(), e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("Erro: " + e.ToString(), e);
                }
                await Conversation.SendAsync(activity, () => new RootLuisDialog());
                log.Info("Usuário [" + activity.ChannelId + "][" + activity.From.Id + "]: " + activity.Text);
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