using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PortoSeguroBOT.Form_Flows;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PortoSeguroBOT.Dialogs
{
    [LuisModel(Constants.LuisIdSeguro, Constants.LuisSubscriptionKey)]
    [Serializable]
    public class SeguroLuisDialog : LuisDialog<object>
    {
        [LuisIntent("ContratarSeguroViagem")]
        public async Task SeguroViagemAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("[SeguroLuisDialog] Entendi que deseja seguro ViagemY");

            var SeguroForms = new FormDialog<FormSeguroViagem>(new FormSeguroViagem(), FormSeguroViagem.SeguroBuildForm, FormOptions.PromptInStart);
            context.Call(SeguroForms, this.callbackFormViagem);

            //context.Wait(MessageReceived);
        }

        [LuisIntent("None")]   
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("[SeguroLuisDialog] Desculpe, eu não entendi.");
            context.UserData.SetValue("SourceDialog", "SeguroLuisDialog");
            await context.Forward(new RootLuisDialog(), null, new Activity { Text = userToBotText }, System.Threading.CancellationToken.None);            
        }

        private async Task callbackFormViagem(IDialogContext context, IAwaitable<object> result)
        {
            //var reply = context.MakeMessage();
            //reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            //reply.Attachments = GetCardsAttachments();
            await context.PostAsync("[SeguroLuisDialog] Voltei do Formulário de Viagem");
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