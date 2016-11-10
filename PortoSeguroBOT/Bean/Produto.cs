using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}