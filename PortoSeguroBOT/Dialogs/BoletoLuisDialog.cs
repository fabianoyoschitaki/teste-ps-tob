using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PortoSeguroBOT.Dialogs
{
    [LuisModel(Constants.LuisIdBoleto, Constants.LuisSubscriptionKey)]
    [Serializable]
    public class BoletoLuisDialog : LuisDialog<object>
    {
        [LuisIntent("GerarSegundaViaBoleto")]
        public async Task GerarSegundaViaBoletoAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("[BoletoLuisDialog] Entendi que deseja gerar segunda via boleto");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("[BoletoLuisDialog] Desculpe, eu não entendi.");
            context.UserData.SetValue("SourceDialog", "BoletoLuisDialog");
            await context.Forward(new RootLuisDialog(), null, new Activity { Text = userToBotText }, System.Threading.CancellationToken.None);            
        }

        private string userToBotText;
        protected async override Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            userToBotText = (await item).Text;
            await base.MessageReceived(context, item);
        }
    }
}