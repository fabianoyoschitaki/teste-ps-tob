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
                //string UserFirstName = Formatters.Capitalize(user.Nome).Split()[0];
                //PromptDialog.Text(context, callbackConfirmaData, $"{UserFirstName}, para continuarmos nos confirme sua data de nascimento. Caso deseje a segunda via para outro CPF digite OUTRO");
                ShowUserProducts(context, user);
                context.Wait(MessageReceived);
            }
            else
            {
                PromptDialog.Text(context, callbackBoletoCPF, "Para continuarmos com a solicitação de segunda via de boleto digite seu CPF ou CNPJ");
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
                    if (Validators.IsCpf(textResult) || Validators.IsCnpj(textResult))
                    {
                        Usuario user = null;
                        if (Validators.IsCpf(textResult))
                        {
                            user = Usuario.GetUsuario(textResult,true);
                        }
                        else
                        {
                            user  = Usuario.GetUsuario(textResult,false);
                        }

                       if(user.Nome != null)
                        {
                            context.UserData.SetValue("UserData", user);
                            string UserFirstName = Formatters.Capitalize(user.Nome).Split()[0];
                            if (Validators.IsCpf(textResult)) {
                                PromptDialog.Text(context, callbackConfirmaData, $"{UserFirstName}, qual a sua data de Nascimento?");
                            }
                            else
                            {
                                ShowUserProducts(context, user);
                                context.Wait(MessageReceived);
                            }
                            
                        }
                        else
                        {
                            PromptDialog.Text(context, callbackBoletoCPF, "Desculpe, não encontramos esse CPF/CNPJ em nossa base de dados, digite novamente o CPF/CNPJ ou digite SAIR para cancelar a solicitação de segunda via.");
                        }
                    }
                    else
                    {
                        PromptDialog.Text(context, callbackBoletoCPF, "Desculpe, o texto digitado não é um CPF/CNPJ válido, digite novamente o CPF/CNPJ ou SAIR para cancelar a solicitação de segunda via.");
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
                    PromptDialog.Text(context, callbackBoletoCPF, "Digite o CPF/CNPJ para a emissão de segunda via");
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
                                ShowUserProducts(context, user);
                                context.Wait(MessageReceived);
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

        private async Task callbackParcelaEmail(IDialogContext context, IAwaitable<string> result)
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
                    try
                    {
                        string bolCode;
                        if(Validators.IsMail(textResult))
                        {
                            if (context.UserData.TryGetValue("SelParc", out bolCode))
                            {
                                dynamic ret = new Produto().SendEmailBoletoRE(bolCode,textResult);
                                if(ret != null)
                                {
                                    if("Boleto enviado!".Equals(ret.retorno.mensagem.Value))
                                    {
                                        await context.PostAsync("O boleto foi enviado por email e deve chegar dentro dos próximos minutos. Podemos te ajudar em algo mais?");
                                    } else
                                    {
                                        await context.PostAsync("Ocorreu um erro no sistema. Por favor, tente novamente mais tarde.");
                                    }
                                }
                                else
                                {
                                    await context.PostAsync("Ocorreu um erro no sistema. Por favor, tente novamente mais tarde.");
                                }
                                context.Wait(MessageReceived);
                            }
                            else
                            {
                                await context.PostAsync("Ocorreu um erro no sistema.");
                                context.Wait(MessageReceived);
                            }
                        }
                        else
                        {
                            PromptDialog.Text(context, callbackParcelaEmail, $"Desculpe, o email digitado não é um email válido. Digite novamente ou digite SAIR para cancelar a solicitação.");
                        }
                    }
                    catch (Exception e)
                    {
                        await context.PostAsync("Desculpe, ocorreu um erro. Tente novamente mais tarde, podemos te ajudar em mais alguma coisa?");
                        context.Wait(MessageReceived);
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
            }
        }

        public async void ShowUserProducts(IDialogContext context, Usuario user)
        {
            //await context.PostAsync("Vamos listar seus produtos Porto Seguro [Até Aqui]");
            if (user.Produtos != null)
            {
                try
                {
                    await context.PostAsync("Selecione entre seus produtos qual deseja solicitar segunda via: ");
                    List<Attachment> heroCards = new List<Attachment>();
                    foreach (Produto prod in user.Produtos)
                    {
                        string Dados = "";
                        Dados += prod.Sucursal != null ? prod.Sucursal + "-" : "";
                        Dados += prod.Ramo != null ? prod.Ramo + "-" : "";
                        Dados += prod.NumeroApolice;
                        Dados += prod.Item != null ? "-" + prod.Item : "";
                        string imgUrl = "";
                        switch (prod.Codigo)
                        {
                            case 1:
                                //imgUrl = "https://cliente.portoseguro.com.br/static-files/images/Seguro-Auto-atendimento-portal.jpg";
                                imgUrl = "";
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
                            new CardAction(ActionTypes.PostBack, "Solicitar 2ª Via", value: "ProdCod|" + prod.Codigo.ToString() + "|" + prod.Sucursal.ToString() + "|" + prod.Ramo.ToString() + "|" + prod.NumeroApolice.ToString() + "|" + prod.Item.ToString()))
                        );
                    }
                    var reply = context.MakeMessage();
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments = heroCards;
                    await context.PostAsync(reply);
                }
                catch (Exception e)
                {
                    await context.PostAsync("Desculpe, ocorreu um erro ao listar seus produtos, tente novamente mais tarde.");
                }
            }
            
        }

        public async void ShowParcelasRE(IDialogContext context, dynamic Documento)
        {
            //await context.PostAsync("Vamos listar seus produtos Porto Seguro [Até Aqui]");
            if (Documento != null)
            {
                try
                {
                    await context.PostAsync("Selecione a parcela que deseja emitir o boleto");
                    List<Attachment> heroCards = new List<Attachment>();

                    foreach (dynamic parc in Documento.documento.parcelas.parcela)
                    {
                        heroCards.Add(GetHeroCard(
                                "Parcela" + parc.numeroParcela.Value,
                                "Valor: " + parc.valorLiquidoParcela.Value,
                                "",
                                new CardImage(url: ""),
                                new CardAction(ActionTypes.PostBack, "Emitir Boleto", value: "SelBoleto|" + Documento.documento.codigoOrigemProposta.Value + "|" + Documento.documento.numeroDigitoProposta.Value + "|" + parc.numeroParcela.Value))
                        );
                    }
                    var reply = context.MakeMessage();
                    reply.AttachmentLayout = AttachmentLayoutTypes.List;
                    reply.Attachments = heroCards;
                    await context.PostAsync(reply);
                }
                catch (Exception e)
                {
                    await context.PostAsync("Ocorreu um erro ao gerar as parcelas para seleção. Tente novamente mais tarde.");
                }
            }
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

        public async void GerarSegundaViaBoleto(IDialogContext context, string codigo)
        {
            await context.PostAsync($"Aguarde um momento enquanto verificamos seu boleto.");
            try
            {
                if (codigo.Split('|')[1] == "1")
                {
                    string urlToPdf = new Produto().getProdutoSegundaViaURL(codigo);
                    if (urlToPdf == null)
                    {
                        await context.PostAsync("Sua apólice não foi emitida com pagamento via boleto ou não tem um boleto em aberto para pagamento.");
                    }
                    else
                    {
                        await context.PostAsync($"Faça o download do seu boleto clicando aqui: {urlToPdf}");
                    }
                }
                else if(codigo.Split('|')[1] == "2")
                {
                    dynamic documentoRE = new Produto().getProdutoSegundaViaRE(codigo);
                    if(documentoRE != null)
                    {
                        ShowParcelasRE(context, documentoRE);
                        //await context.PostAsync($"Encontrei as parcelas");
                    }
                }
                else
                {
                    await context.PostAsync($"Desculpe, no momento não consigo emitir segunda via para esse tipo de produto.");
                }
              
            }
            catch (Exception e)
            {
                await context.PostAsync($"Ocorreu um erro ao gerar a segunda via de seu boleto.");
            }
            //await context.PostAsync("Podemos te ajudar em mais alguma coisa?");
        }

        public async Task ParcelaBoletoRESelecionada(IDialogContext context, string codigo)
        {
            context.UserData.SetValue("SelParc", codigo);
            PromptDialog.Text(context, callbackParcelaEmail, "Para esse tipo de apólice o boleto é enviado via email. Qual o seu email?");
        }

        private string userToBotText;
        protected async override Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            userToBotText = (await item).Text;

            string codeSelected = userToBotText;
            if (codeSelected.IndexOf("ProdCod") != -1)
            {
                GerarSegundaViaBoleto(context, codeSelected);
                context.Wait(MessageReceived);
            }
            else if(codeSelected.IndexOf("SelBoleto") != -1)
            {
                await ParcelaBoletoRESelecionada(context, codeSelected);
            }
            else
            {
                await base.MessageReceived(context, item);
            }
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