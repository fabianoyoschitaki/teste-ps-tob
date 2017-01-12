using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PortoSeguroBOT.QnA
{
    /// <summary>
    /// Classe responsável por acionar FAQ http://atendimento.portoseguro.com.br/consulta-de-cliente/faq-seguro-auto/
    /// </summary>
    public class QnACaller
    {
        //log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static dynamic obterResposta(QnAEnum.BASE source, string pergunta)
        {
            StringBuilder URL = new StringBuilder(QnAConstants.QNA_HOST);
            switch (source)
            {
                case QnAEnum.BASE.CARTAO_PORTO_SEGURO:
                    URL.Append(QnAConstants.QNA_CARTAO_PORTO_SEGURO_URL);
                    break;

                case QnAEnum.BASE.SEGURO_AUTO:
                    URL.Append(QnAConstants.QNA_SEGURO_AUTO_URL);
                    break;

                case QnAEnum.BASE.CONSORCIO_IMOVEL:
                    URL.Append(QnAConstants.QNA_CONSORCIO_IMOVEL_URL);
                    break;

                case QnAEnum.BASE.CAPITALIZACAO:
                    URL.Append(QnAConstants.QNA_CAPITALIZACAO_URL);
                    break;

                case QnAEnum.BASE.SEGURO_ALUGUEL:
                    URL.Append(QnAConstants.QNA_SEGURO_ALUGUEL_URL);
                    break;

                case QnAEnum.BASE.CONSORCIO_AUTOMOVEL:
                    URL.Append(QnAConstants.QNA_CONSORCIO_AUTOMOVEL_URL);
                    break;
            }           

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers["Ocp-Apim-Subscription-Key"] = QnAConstants.QNA_SUBSCRIPTION_KEY;
            request.Method = "POST";
            dynamic objJson = null;
            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"question\":\"" + pergunta + "\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse) request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    objJson = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                log.Error("Erro: " + e.ToString(), e);
            }
            return objJson;
        }
    }
}