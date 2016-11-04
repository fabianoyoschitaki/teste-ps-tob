using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text;

namespace PortoSeguroBOT.Bean
{
    public class SeguroViagem
    {
        public string UfOrigem { get; set; }
        public string EstadoOrigem { get; set; }
        public string PaisDestino { get; set; }
        public int CodPaisDestino { get; set; }
        public int CodContinente { get; set; }
        public Boolean PaisEuropeuDestino { get; set; }
        public DateTime DataPartida { get; set; }
        public DateTime DataRetorno { get; set; }
        public int Menor70 { get; set; }
        public int Maior70 { get; set; }
        public Boolean EsporteAventura { get; set; }
        public string Motivo { get; set; }
        public int CodMotivo { get; set; }


        public class DataObject
        {
            public string jsonResult { get; set; }
        }


        private dynamic GetCotacaoSeguroViagem()
        {

            StringBuilder URL = new StringBuilder();
            URL.Append("https://wwws.portoseguro.com.br/vendaonline/viagem/valorseguroajax.ns?codigoAtendimento=&calculo.isObjetoClonado=false&codigoOperacao=525-A&calculo.codigoOperacao=525-A&codigoCanal=1&codigoCampanha=99999&codigoSusepCorretor=&calculo.susepCorretorWeb=&calculo.browserType=Mozilla%2F5.0+(Windows+NT+6.1%3B+WOW64)+AppleWebKit%2F537.36+(KHTML%2C+like+Gecko)+Chrome%2F50.0.2661.102+Safari%2F537.36");
            URL.Append("&calculo.codigoUfOrigem=");
            URL.Append(this.UfOrigem);
            URL.Append("&calculo.descricaoUfOrigem=");
            URL.Append(this.EstadoOrigem);
            URL.Append("&calculo.codigoPaisDestino=");
            URL.Append(this.CodPaisDestino.ToString());
            URL.Append("&calculo.codigoContinenteDestino=");
            URL.Append(this.CodContinente.ToString());
            URL.Append("&calculo.descricaoPaisDestino=");
            URL.Append(this.PaisDestino);
            URL.Append("&calculo.dataSaidaProponente=25%2F11%2F2016");
            URL.Append("&calculo.diaSaidaProponente=25");
            URL.Append("&calculo.mesSaidaProponente=11");
            URL.Append("&calculo.anoSaidaProponente=2016");
            URL.Append("&calculo.dataRetornoProponente=28%2F11%2F2016");
            URL.Append("&calculo.diaRetornoProponente=28");
            URL.Append("&calculo.mesRetornoProponente=11");
            URL.Append("&calculo.anoRetornoProponente=2016");
            URL.Append("&calculo.duracaoViagem=4");
            URL.Append("&calculo.qtdAcompanhantes=0");
            URL.Append("&calculo.qtdPassagMenorSetentaAnos=1");
            URL.Append("&calculo.qtdPassagMaiorSetentaAnos=0");
            URL.Append("&isVisitaPaisEuropeu=");
            URL.Append("&calculo.isPraticaAventura=false");
            URL.Append("&calculo.motivoViagem=1");

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";

            dynamic objJson;

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string JSON = reader.ReadToEnd();
                objJson = JsonConvert.DeserializeObject(JSON);
                
            }

            return objJson;
        }
        public IList<Attachment> GetCardsAttachments()
        {
            dynamic dadosCotacao = GetCotacaoSeguroViagem();
            List<Attachment> heroCards = new List<Attachment>();
            foreach(var data in dadosCotacao.retornoCalculo.listaPacotesContratacao)
            {
                heroCards.Add(GetHeroCard(
                        data.nomePlano.Value,
                        "6x de R$" + data.formasParcelamento.listaParcCartao[5].valorPrimeiraParcela.Value,
                         "Ou à vista R$" + data.formasParcelamento.listaParcCartao[0].valorPrimeiraParcela.Value,
                        new CardImage(url: ""),
                        new CardAction(ActionTypes.OpenUrl, "Contratar", value: "https://wwws.portoseguro.com.br/vendaonline/viagem/home.ns#" + data.codigoPlano.Value))
                );
            }
            return heroCards;
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