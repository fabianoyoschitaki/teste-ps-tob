using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PortoSeguroBOT.Bean;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly log4net.ILog logNone = log4net.LogManager.GetLogger("BotRollingFileLoggerNone");

        [LuisIntent("GerarSegundaViaBoleto")]
        [LuisIntent("GerarSegundaViaBoletoRE")]
        [LuisIntent("GerarSegundaViaBoletoSaude")]
        [LuisIntent("GerarSegundaViaBoletoCredito")]
        [LuisIntent("GerarSegundaViaBoletoAuto")]
        public async Task GerarSegundaViaBoletoAsync(IDialogContext context, LuisResult result)
        {
            Usuario user = new Usuario();
            if (context.UserData.TryGetValue("UserData", out user))
            {
                string UserFirstName = Formatters.Capitalize(user.Nome).Split()[0];
                PromptDialog.Text(context, callbackConfirmaData, $"{UserFirstName}, para continuarmos nos confirme sua data de nascimento. Caso deseje a segunda via para outro CPF digite OUTRO");
            }
            else
            {
                PromptDialog.Text(context, callbackBoletoCPF, "Para continuarmos com a solicitação de segunda via de boleto digite seu CPF");
            }
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
                            context.UserData.SetValue("UserData", user);
                            string UserFirstName = Formatters.Capitalize(user.Nome).Split()[0];
                            PromptDialog.Text(context, callbackConfirmaData, $"{UserFirstName}, qual a sua data de Nascimento?");
                        }
                        else
                        {
                            await context.PostAsync("Cliente não encontrado");
                            context.Wait(MessageReceived);
                        }
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

        private async Task callbackConfirmaData(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
               var textResult = await result;
                if ("SAIR".Equals(textResult, StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync("Sua solicitação de boleto foi cancelada, como podemos te ajudar?");
                    context.Wait(MessageReceived);
                }
                else if ("OUTRO".Equals(textResult, StringComparison.InvariantCultureIgnoreCase))
                {
                    PromptDialog.Text(context, callbackBoletoCPF, "No momento só conseguimos emitir segunda via de boleto do produto Residência, caso deseje digite seu CPF");
                }
                else
                {
                  try
                    {
                        DateTime dt = DateTime.ParseExact(textResult, "dd/MM/yyyy", null);
                        Usuario user = new Usuario();
                        if(context.UserData.TryGetValue("UserData", out user))
                        {
                            if (user.DataNasc.Date.Equals(dt.Date))
                            {
                                //await context.PostAsync("Vamos listar seus produtos Porto Seguro [Até Aqui]");
                                if (user.Produtos != null)
                                {
                                    await context.PostAsync("Selecione entre seus produtos qual deseja solicitar segunda via: ");
                                    List<Attachment> heroCards = new List<Attachment>();
                                    foreach (Produto prod in user.Produtos)
                                    {
                                        string Dados = "";
                                        Dados += prod.Sucursal != null ? prod.Sucursal + "-":"";
                                        Dados += prod.Ramo != null ? prod.Ramo + "-" : "";
                                        Dados += prod.NumeroApolice;
                                        Dados += prod.Item != null ? "-" + prod.Item:"";
                                        string imgUrl = "";
                                        switch(prod.Codigo)
                                        {
                                            case 1:
                                                imgUrl = "https://cliente.portoseguro.com.br/static-files/images/Seguro-Auto-atendimento-portal.jpg";
                                                break;
                                            default:
                                                imgUrl = "";
                                                break;
                                        }
                                        heroCards.Add(GetHeroCard(
                                            prod.Nome,
                                            Dados,
                                            "",
                                            new CardImage(url: imgUrl),
                                            new CardAction(ActionTypes.ImBack, "Solicitar 2ª Via", value: prod.Codigo.ToString()))
                                        );
                                    }
                                    var reply = context.MakeMessage();
                                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                                    reply.Attachments = heroCards;
                                    await context.PostAsync(reply);
                                }
                                context.Wait(ProductSelectionCallback);

                            }
                            else
                            {
                                PromptDialog.Text(context, callbackConfirmaData, "Desculpe, a data informada não é a mesma do cadastro, digite novamente no formato dd/MM/yyyy ou SAIR para cancelar a solicitação de segunda via.");
                            }
                        }
                        else
                        {
                            await context.PostAsync("Ocorreu um erro no sistema.");
                            context.Wait(MessageReceived);
                        }
                    }
                    catch (Exception e)
                    {
                        PromptDialog.Text(context, callbackConfirmaData, $"Desculpe '{textResult}' não é uma data válida, digite no formato dd/MM/yyyy ou SAIR para cancelar a solicitação de segunda via.");
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
             }
        }

        protected async Task ProductSelectionCallback(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            string codeSelected = (await item).Text;
            await context.PostAsync($"Você selecionou a Apólice: {codeSelected}");
            await base.MessageReceived(context, item);
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            logNone.Info(this.GetType().Name + "-None: " + userToBotText);
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

        public Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };
            return heroCard.ToAttachment();
        }
    }
}