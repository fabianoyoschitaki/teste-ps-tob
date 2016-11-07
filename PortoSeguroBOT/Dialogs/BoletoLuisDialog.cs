using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PortoSeguroBOT.Bean;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            PromptDialog.Text(context, callbackBoletoCPF, "No momentos só conseguimos emitir segunda via de boleto do produto Residência, caso deseje digite seu CPF");
        }

        private async Task callbackBoletoCPF(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var textResult = await result;
                if ("SAIR".Equals(textResult, StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync("Sua solicitação de boleto foi cancelada, como podemos te ajudar?");
                    context.Wait(MessageReceived);
                }
                else
                {
                    if (Validators.IsCpf(textResult))
                    {
                        Usuario user = Usuario.GetUsuario(textResult);
                        if(user.Nome != null)
                        {
                            await context.PostAsync($"Seu Nome é: {user.Nome}");
                            await context.PostAsync($"Data de Nascimento: {user.DataNasc}");
                            context.UserData.SetValue("CPFUser", textResult);
                        }
                        else
                        {
                            await context.PostAsync("Cliente não encontrado");
                        }
                      
                        context.Wait(MessageReceived);
                    }
                    else
                    {
                        PromptDialog.Text(context, callbackBoletoCPF, "Desculpe, o texto digitado não é um CPF válido, digite novamente o CPF ou SAIR para cancelar a solicitação de segunda via.");
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
            }
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("[BoletoLuisDialog] Desculpe, eu não entendi.");
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