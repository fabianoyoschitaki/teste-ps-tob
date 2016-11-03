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

namespace PortoSeguroBOT.Bean
{
    public class SeguroViagem
    {
        public string UfOrigem { get; set; }
        public string EstadoOrigem { get; set; }
        public string PaisDestino { get; set; }
        public int CodPaisDestino { get; set; }
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

            string URL = "https://wwws.portoseguro.com.br/vendaonline/viagem/valorseguroajax.ns?codigoAtendimento=&calculo.isObjetoClonado=false&codigoOperacao=525-A&calculo.codigoOperacao=525-A&codigoCanal=1&codigoCampanha=99999&codigoSusepCorretor=&calculo.susepCorretorWeb=&calculo.browserType=Mozilla%2F5.0+(Windows+NT+6.1%3B+WOW64)+AppleWebKit%2F537.36+(KHTML%2C+like+Gecko)+Chrome%2F50.0.2661.102+Safari%2F537.36&calculo.codigoUfOrigem=SP&calculo.descricaoUfOrigem=S%C3%A3o+Paulo&calculo.codigoPaisDestino=233&calculo.codigoContinenteDestino=3&calculo.descricaoPaisDestino=Portugal&calculo.dataSaidaProponente=25%2F11%2F2016&calculo.diaSaidaProponente=25&calculo.mesSaidaProponente=11&calculo.anoSaidaProponente=2016&calculo.dataRetornoProponente=28%2F11%2F2016&calculo.diaRetornoProponente=28&calculo.mesRetornoProponente=11&calculo.anoRetornoProponente=2016&calculo.duracaoViagem=4&calculo.qtdAcompanhantes=0&calculo.qtdPassagMenorSetentaAnos=1&calculo.qtdPassagMaiorSetentaAnos=0&destino-pais=233%7C3&calculo.codigoPaisDestinoAdicional2=&calculo.codigoContinenteDestinoAdicional2=&calculo.descricaoPaisDestinoAdicional2=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional3=&calculo.codigoContinenteDestinoAdicional3=&calculo.descricaoPaisDestinoAdicional3=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional4=&calculo.codigoContinenteDestinoAdicional4=&calculo.descricaoPaisDestinoAdicional4=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional5=&calculo.codigoContinenteDestinoAdicional5=&calculo.descricaoPaisDestinoAdicional5=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional6=&calculo.codigoContinenteDestinoAdicional6=&calculo.descricaoPaisDestinoAdicional6=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional7=&calculo.codigoContinenteDestinoAdicional7=&calculo.descricaoPaisDestinoAdicional7=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional8=&calculo.codigoContinenteDestinoAdicional8=&calculo.descricaoPaisDestinoAdicional8=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional9=&calculo.codigoContinenteDestinoAdicional9=&calculo.descricaoPaisDestinoAdicional9=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional10=&calculo.codigoContinenteDestinoAdicional10=&calculo.descricaoPaisDestinoAdicional10=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional11=&calculo.codigoContinenteDestinoAdicional11=&calculo.descricaoPaisDestinoAdicional11=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional12=&calculo.codigoContinenteDestinoAdicional12=&calculo.descricaoPaisDestinoAdicional12=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional13=&calculo.codigoContinenteDestinoAdicional13=&calculo.descricaoPaisDestinoAdicional13=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional14=&calculo.codigoContinenteDestinoAdicional14=&calculo.descricaoPaisDestinoAdicional14=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional15=&calculo.codigoContinenteDestinoAdicional15=&calculo.descricaoPaisDestinoAdicional15=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional16=&calculo.codigoContinenteDestinoAdicional16=&calculo.descricaoPaisDestinoAdicional16=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional17=&calculo.codigoContinenteDestinoAdicional17=&calculo.descricaoPaisDestinoAdicional17=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional18=&calculo.codigoContinenteDestinoAdicional18=&calculo.descricaoPaisDestinoAdicional18=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional19=&calculo.codigoContinenteDestinoAdicional19=&calculo.descricaoPaisDestinoAdicional19=&isVisitaPaisEuropeu=&calculo.codigoPaisDestinoAdicional20=&calculo.codigoContinenteDestinoAdicional20=&calculo.descricaoPaisDestinoAdicional20=&isVisitaPaisEuropeu=&calculo.isVisitaPaisEuropeu=S&calculo.isPraticaAventura=false&calculo.motivoViagem=1&calculo.origem=&rand=0.5719053450579579";

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
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