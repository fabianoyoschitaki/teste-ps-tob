using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace PortoSeguroBOT.Bean
{
    public class Usuario
    {
        public string NumeroCPF { get; set; }
        public string DigitoCPF { get; set; }
        public string Nome { get; set; }
        public DateTime DataNasc { get; set; }
        public IList<Produto> Produtos {get;set;}

        public static Usuario GetUsuario(string CPF)
        {
            CPF = CPF.Replace(".","").Replace("-","");
            if (CPF.Length == 10) CPF = "0" + CPF;
            Usuario user = new Usuario();
            user.NumeroCPF = CPF.Substring(0, 9);
            user.DigitoCPF = CPF.Substring(9, 2);
            dynamic dados = GetUserData(user.NumeroCPF, user.DigitoCPF);
            try { 
                user.Nome = dados.Envelope.Body.obterDocumentosPorCnpjCpfResponse.obterDocumentosPorCnpjCpfReturn.nomePessoa["#text"].Value;
                user.DataNasc = dados.Envelope.Body.obterDocumentosPorCnpjCpfResponse.obterDocumentosPorCnpjCpfReturn.dataNascimento["#text"].Value;
                IList<Produto> prods = new List<Produto>();
                dynamic prodList = dados.Envelope.Body.obterDocumentosPorCnpjCpfResponse.obterDocumentosPorCnpjCpfReturn.documentoSeguradoServiceVO;
                foreach (dynamic doc in prodList)
                {
                    Produto p = new Produto();
                    p.Codigo = Convert.ToInt32(doc.codigoProdutoUnificacao.Value);
                    p.Nome = doc.produto.Value;
                    if(p.Codigo == 5) {
                        p.NumeroApolice = Convert.ToInt32(doc.chaveDocumentoVO.valorDocumento.Value);
                    }
                    else
                    {
                        foreach (dynamic key in doc.chaveDocumentoVO)
                        {
                            if ("sucursal".Equals(key.chaveDocumento.Value)) p.Sucursal = Convert.ToInt32(key.valorDocumento.Value);
                            else if ("ramo".Equals(key.chaveDocumento.Value)) p.Ramo = Convert.ToInt32(key.valorDocumento.Value);
                            else if ("apolice".Equals(key.chaveDocumento.Value)) p.NumeroApolice = Convert.ToInt32(key.valorDocumento.Value);
                            else if ("item".Equals(key.chaveDocumento.Value)) p.Item = Convert.ToInt32(key.valorDocumento.Value);
                        }
                    }
                   
                    prods.Add(p);
                }
                user.Produtos = prods;
                return user;
            } catch (Exception e)
            {
                string erro = e.ToString();
                user.NumeroCPF = null;
                user.DigitoCPF = null;
                return user;
            }
        }

        public static dynamic GetUserData(string NumeroCPF,string DigitoCPF)
        {
            var _url = "https://wwws.portoseguro.com.br/consultadadoapolicesegurado/services/ConsultaDadoSeguradoService";
            var _action = "https://wwws.portoseguro.com.br/consultadadoapolicesegurado/services/ConsultaDadoSeguradoService?op=obterDocumentosPorCnpjCpf";

            XmlDocument soapEnvelopeXml = new XmlDocument();
            string SoapRequest = @"<soapenv:Envelope xmlns:soapenv = ""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://service.consultadadoapolicesegurado.auto.porto.com""><soapenv:Header/><soapenv:Body> <ser:obterDocumentosPorCnpjCpf><ser:numCnpjCpf>";
            SoapRequest += NumeroCPF;
            SoapRequest += @"</ser:numCnpjCpf><ser:digCnpjCpf>";
            SoapRequest += DigitoCPF;
            SoapRequest += @"</ser:digCnpjCpf></ser:obterDocumentosPorCnpjCpf></soapenv:Body ></soapenv:Envelope>";
            soapEnvelopeXml.LoadXml(SoapRequest);

            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            try
            {
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        var xDoc = XDocument.Parse(RemoveAllNamespaces(soapResult));
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xDoc.ToString());
                        string JsonResult = JsonConvert.SerializeXmlNode(doc);
                        dynamic root = JsonConvert.DeserializeObject(JsonResult);
                        return root;
                    }
                    
                }
            }
            catch(WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();
                            var xDoc = XDocument.Parse(RemoveAllNamespaces(error));
                            dynamic root = new ExpandoObject();
                            Parse(root, xDoc.Elements().First());
                            return root;
                        }
                    }
                }
            }
            return null;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        public static void Parse(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    //list
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        Parse(list, element);
                    }

                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    //element
                    foreach (var element in node.Elements())
                    {
                        Parse(item, element);
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }

        //Implemented based on interface, not part of algorithm
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
    }
}