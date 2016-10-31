using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
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
            await context.PostAsync("Entendi que deseja seguro ViagemX");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Desculpe, eu não entendi...");
        }
    }
}