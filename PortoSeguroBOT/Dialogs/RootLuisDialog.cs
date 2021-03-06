﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PortoSeguroBOT.Bean;
using PortoSeguroBOT.Helpers;
using PortoSeguroBOT.QnA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using static PortoSeguroBOT.QnA.QnAEnum;

namespace PortoSeguroBOT.Dialogs
{
    [LuisModel(Constants.LuisIdRoot, Constants.LuisSubscriptionKey)]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog logNone = log4net.LogManager.GetLogger("BotRollingFileLoggerNone");

        [LuisIntent("ContratarSeguro")]
        [LuisIntent("HealthForPets")]
        [LuisIntent("SolicitarBoleto")]
        [LuisIntent("Saudacao")]
        [LuisIntent("Despedida")]
        [LuisIntent("Negacao")]
        public async Task RootAsync(IDialogContext context, LuisResult result)
        {
            switch (result.Intents[0].Intent)
            {
                case "ContratarSeguro":
                    await this.SeguroAsync(context, result);
                    break;
                case "SolicitarBoleto":
                    await this.BoletoAsync(context, result);
                    break;
                case "Saudacao":
                    await this.SaudacaoAsync(context, result);
                    break;
                case "Despedida":
                    await this.DespedidaAsync(context, result);
                    break;
                case "Negacao":
                    await this.DespedidaAsync(context, result);
                    break;
                case "HealthForPets":
                    await this.HealthForPetsAsync(context, result);
                    break;
            }
        }

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

        public async Task SaudacaoAsync(IDialogContext context, LuisResult result)
        {
            Usuario user = new Usuario();
            if (context.UserData.TryGetValue("UserData", out user))
            {
                string UserFirstName = Formatters.Capitalize(user.Nome).Split()[0];
                await context.PostAsync($"Benvindo a Porto Seguro {UserFirstName}, como podemos te ajudar?");
            }
            else
            {
                await context.PostAsync("Benvindo a Porto Seguro, como podemos te ajudar?");
            }

            context.Wait(MessageReceived);
        }

        public async Task DespedidaAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ok, até mais! Se precisar de alguma coisa é só falar!");
            context.Wait(MessageReceived);
        }

        public async Task HealthForPetsAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Esperamos que esteja tudo bem com seu PET. Saiba mais sobre saúde para o seu pet no Portal do Health For Pets");
            List<Attachment> heroCards = new List<Attachment>();

            var heroCard = new HeroCard
            {
                Title = "Health For Pets",
                Subtitle = "Nossa missão é proporcionar a cães e gatos uma vida mais saudável e feliz.",
                Text = "Acesse e saiba mais",
                //Images = new List<CardImage>() { new CardImage(url: "https://health4pet.com.br/static/images/header_logo.png") },
                Images = new List<CardImage>() { new CardImage(url: "") },
                Buttons = new List<CardAction>() { new CardAction(ActionTypes.OpenUrl, "Acesse", value: "https://health4pet.com.br/") },
            };
            heroCards.Add(heroCard.ToAttachment());

            var reply = context.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            reply.Attachments = heroCards;
            
            await context.PostAsync(reply);
            await context.PostAsync("Podemos te ajudar em mais alguma coisa?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("TrocarUsuario")]
        public async Task TrocarUsuarioAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("No momento eu consigo cotar um seguro para sua viagem e emitir uma segunda via de boleto para o seguro do seu auto. O que deseja fazer?");
            context.Wait(MessageReceived);
        }


        [LuisIntent("habilidadesBot")]
        public async Task habilidadesAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("No momento eu consigo cotar um seguro para sua viagem e emitir uma segunda via de boleto para o seguro do seu auto. O que deseja fazer?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("SolicitarInformacao")]
        public async Task SolicitarInfoAsync(IDialogContext context, LuisResult result)
        {
            //solicitarInformacao(context, result);
            try
            {
                string searchFor = "";
                foreach (var dt in result.Entities)
                {
                    if ("InfoEntity".Equals(dt.Type))
                    {
                        searchFor = dt.Entity;
                    }
                }
                dynamic resposta = QnACaller.obterResposta(QnAEnum.BASE.SEGURO_AUTO, searchFor);
                string retorno = WebUtility.HtmlDecode(resposta.answer.Value);
                await context.PostAsync(retorno);
            }
            catch (Exception e)
            {
                await context.PostAsync("Desculpe, ocorreu um erro enquanto buscávamos mais informações, por favor tente mais tarde.");
            }
            context.Wait(MessageReceived);
        }

        private async void solicitarInformacao(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Por favor, veja se algum desses links pode te ajudar:");
            try
            {
                string SearchFor = "";
                foreach (var dt in result.Entities)
                {
                    if ("InfoEntity".Equals(dt.Type))
                    {
                        SearchFor = dt.Entity;
                    }
                }

                List<SearchResult> SearchList = new List<SearchResult>(PortoSearch.GetPortoSearch(SearchFor));
                SearchList.AddRange(MicrosftAPI.BingSearch(SearchFor));

                List<Attachment> heroCards = new List<Attachment>();

                foreach (SearchResult s in SearchList)
                {
                    heroCards.Add(Formatters.GetHeroCard(
                        s.Title,
                        s.Desc,
                        s.SubDesc,
                        new CardImage(url: ""),
                        new CardAction(ActionTypes.OpenUrl, "Acessar", value: s.Link))
                    );
                }

                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = heroCards;
                await context.PostAsync(reply);
            }
            catch (Exception e)
            {
                await context.PostAsync("Desculpe, ocorreu um erro enquanto buscávamos mais informações, por favor tente mais tarde.");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            logNone.Info(this.GetType().Name + "-None: " + userToBotText);
            await context.PostAsync("Desculpe, eu não entendi...");
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