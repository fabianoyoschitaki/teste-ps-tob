using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.Helpers
{
    public class ConstantsUF
    {
        public static readonly Estado[] ESTADOS = new Estado[]
        {
            new Estado("AC", "Acre"),
            new Estado("AL", "Alagoas"),
            new Estado("AM", "Amazonas"),
            new Estado("AP", "Amapá"),
            new Estado("BA", "Bahia"),
            new Estado("CE", "Ceará"),
            new Estado("DF", "Distrito Federal"),
            new Estado("ES", "Espírito Santo"),
            new Estado("GO", "Goiás"),
            new Estado("MA", "Maranhão"),
            new Estado("MG", "Minas Gerais"),
            new Estado("MS", "Mato Grosso do Sul"),
            new Estado("MT", "Mato Grosso"),
            new Estado("PA", "Pará"),
            new Estado("PB", "Paraíba"),
            new Estado("PE", "Pernambuco"),
            new Estado("PI", "Piauí"),
            new Estado("PR", "Paraná"),
            new Estado("RJ", "Rio de Janeiro"),
            new Estado("RN", "Rio Grande do Norte"),
            new Estado("RO", "Rondônia"),
            new Estado("RR", "Roraima"),
            new Estado("RS", "Rio Grande do Sul"),
            new Estado("SC", "Santa Catarina"),
            new Estado("SE", "Sergipe"),
            new Estado("SP", "São Paulo"),
            new Estado("TO", "Tocantins")
        };

        public class Estado
        {
            //TODO arrumar get set
            public string Sigla { get; set; }
            public string Descricao { get; set; }

            public Estado(string Sigla, string Descricao)
            {
                this.Sigla = Sigla;
                this.Descricao = Descricao;
            }

            public override string ToString()
            {
                return String.Concat(Sigla, Descricao);
            }
        }
    }
}