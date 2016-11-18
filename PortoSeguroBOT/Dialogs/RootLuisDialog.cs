using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PortoSeguroBOT.Bean;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog logNone = log4net.LogManager.GetLogger("BotRollingFileLoggerNone");

        [LuisIntent("ContratarSeguro")]
        public async Task SeguroAsync(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("[RootLuisDialog] Entendi que deseja contratar um seguro");
            string sourceDialog;
            if (!context.UserData.TryGetValue("SourceDialog", out sourceDialog)
                || !sourceDialog.Equals("SeguroLuisDialog"))
            {
                await context.Forward(new SeguroLuisDialog(), null, new Activity { Text = userToBotText }, System.Threading.CancellationToken.None);            
            } else
            {
                // se não entendeu, assume
                await this.NoneAsync(context, result);        
            }
        }

        [LuisIntent("SolicitarBoleto")]
        public async Task BoletoAsync(IDialogContext context, LuisResult result)
        {
            string sourceDialog;
            if (!context.UserData.TryGetValue("SourceDialog", out sourceDialog)
                || !sourceDialog.Equals("BoletoLuisDialog"))
            {
                //await context.PostAsync("[RootLuisDialog] Entendi que deseja segunda via de Boleto");
                await context.Forward(new BoletoLuisDialog(), null, new Activity { Text = userToBotText }, System.Threading.CancellationToken.None);
            }
            else
            {
                // se não entendeu, assume
                await this.NoneAsync(context, result);
            }
        }

        [LuisIntent("Saudacao")]
        public async Task SaudacaoAsync(IDialogContext context, LuisResult result)
        {
            Usuario user = new Usuario();
            if (context.UserData.TryGetValue("UserData", out user))
            {
                string UserFirstName = Formatters.Capitalize(user.Nome).Split()[0];
                await context.PostAsync($"Benvindo a Porto Seguro {UserFirstName}, como podemos te ajudar? ");
            }
            else
            {
                await context.PostAsync("Benvindo a Porto Seguro, como podemos te ajudar? ");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Despedida")]
        public async Task DespedidaAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ok, até mais! Se precisar de alguma coisa é só falar!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            logNone.Info(this.GetType().Name + "-None: " + userToBotText);
            //await context.PostAsync($"Não entendeu: {userToBotText}");
            await context.PostAsync("Desculpe, eu não entendi....");
            context.UserData.RemoveValue("SourceDialog");
            context.Wait(MessageReceived);
        }

        private string userToBotText;
        protected async override Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            userToBotText = (await item).Text;
            await base.MessageReceived(context, item);
        }

    }
}