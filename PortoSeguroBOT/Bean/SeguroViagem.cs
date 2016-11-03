using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}