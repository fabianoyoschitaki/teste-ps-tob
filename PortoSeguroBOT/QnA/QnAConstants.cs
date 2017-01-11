using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.QnA
{
    public class QnAConstants
    {
        public const string QNA_HOST = "https://westus.api.cognitive.microsoft.com/qnamaker/v1.0";
        public const string QNA_SUBSCRIPTION_KEY = "7c4293941d9e4257a89288e2de4ea099";

        public const string QNA_SEGURO_AUTO_URL = "/knowledgebases/328abcab-85c6-4025-a23d-6e52a7702548/generateAnswer";
        public const string QNA_CARTAO_PORTO_SEGURO_URL = "/knowledgebases/3919ce11-b2be-4128-aada-a374bcd61fe6/generateAnswer";
        public const string QNA_CONSORCIO_IMOVEL_URL = "/knowledgebases/a34ca150-e3a2-498b-91e3-f187b85d7df1/generateAnswer";
        public const string QNA_CAPITALIZACAO_URL = "/knowledgebases/525ea157-87c9-4f03-ab5b-0b72b6067e9a/generateAnswer";
        public const string QNA_SEGURO_ALUGUEL_URL = "/knowledgebases/c780fcd1-1493-407b-a57a-07b14d3329af/generateAnswer";
        public const string QNA_CONSORCIO_AUTOMOVEL_URL = "/knowledgebases/37479db6-1694-405f-b629-00597addf074/generateAnswer";

    }
}