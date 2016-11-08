using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.Bean
{
    public class Produto
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public int Ramo { get; set; }
        public int Sucursal { get; set; }
        public int NumeroApolice { get; set; }
        public int Item { get; set;}
    }
}