using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PortoSeguroBOT.Bean
{
    public class Produto
    {
        public Int32? Codigo { get; set; }
        public string Nome { get; set; }
        public Int32? Ramo { get; set; }
        public Int32? Sucursal { get; set; }
        public Int32? NumeroApolice { get; set; }
        public Int32? Item { get; set;}

        public Produto() {
            this.Ramo = null;
            this.Sucursal = null;
            this.NumeroApolice = null;
            this.Item = null;
        }

        public string getProdutoSegundaViaURL(string prod)
        {
            string[] prodData = prod.Split('|');
            StringBuilder URL = new StringBuilder();
            URL.Append("https://wwws.portoseguro.com.br/gerenciadorinterfaceweb/bot_boleto.content?sucursal=");
            URL.Append(prodData[2]);
            URL.Append("&ramo=");
            URL.Append(prodData[3]);
            URL.Append("&apolice=");
            URL.Append(prodData[4]);
            URL.Append("&item=");
            URL.Append(prodData[5]);
            URL.Append("&endosso=0");
            
            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";

            dynamic objJson = null;

            try
            {
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string JSON = reader.ReadToEnd();
                    objJson = JsonConvert.DeserializeObject(JSON);

                }
            }
            catch (Exception e)
            {

            }
            return objJson.boleto.urlPdf;
        }

        public dynamic getProdutoSegundaViaRE(string prod)
        {
            string[] prodData = prod.Split('|');
            StringBuilder URL = new StringBuilder();
            URL.Append("https://wwws.portoseguro.com.br/gerenciadorinterfaceweb/bot_boletoRE.content?sucursal=");
            URL.Append(prodData[2]);
            URL.Append("&ramo=");
            URL.Append(prodData[3]);
            URL.Append("&apolice=");
            URL.Append(prodData[4]);
            
            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";

            dynamic objJson = null;
            try
            {
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string JSON = reader.ReadToEnd();
                    objJson = JsonConvert.DeserializeObject(JSON);
                }
            }
            catch (Exception e)
            {

            }
            return objJson;
        }

        public dynamic SendEmailBoletoRE(string codigo,string email)
        {
            string[] prodData = codigo.Split('|');
            StringBuilder URL = new StringBuilder();
            URL.Append("https://wwws.portoseguro.com.br/gerenciadorinterfaceweb/bot_enviaboletoSAP.content?idcontrato=");
            URL.Append(prodData[1]);
            URL.Append(prodData[2]);
            URL.Append("&numeroparcela="); 
            URL.Append(prodData[3].PadLeft(3,'0'));
            URL.Append("&emaildestinatario=");
            URL.Append(email);

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(URL.ToString()) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";

            dynamic objJson = null;
            try
            {
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string JSON = reader.ReadToEnd();
                    objJson = JsonConvert.DeserializeObject(JSON);
                }
            }
            catch (Exception e)
            {

            }
            return objJson;
        }
    }
}