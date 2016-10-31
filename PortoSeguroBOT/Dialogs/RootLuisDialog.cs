using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
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
    [LuisModel(Constants.LuisIdRoot, Constants.LuisSubscriptionKey)]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("ContratarSeguro")]
        public async Task SeguroAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Entendi que deseja contratar um seguro");
            await context.Forward(new SeguroLuisDialog(), null, new Activity { Text = userToBotText }, System.Threading.CancellationToken.None);
        }

        [LuisIntent("SolicitarBoleto")]
        public async Task BoletoAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Entendi que deseja segunda via de Boleto");
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
           await context.PostAsync("Desculpe, eu não entendi...");
        }

        private string userToBotText;
        protected async override Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            userToBotText = (await item).Text;
            await base.MessageReceived(context, item);
        }

    }
}