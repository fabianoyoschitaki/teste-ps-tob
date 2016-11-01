using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.Helpers
{
    public class ConstantsBase
    {
        private static readonly string[] confirm = new string[]{"sim","s","si","yes","yep","yeap","positivo"};
        private static readonly string[] deny = new string[] { "nao","não","n","no","nope","negativo"};

        public static string getConfirmation(string value)
        {
            string retorno = null;
            foreach(string s in confirm)
            {
                if (s.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    retorno = "Sim";
                }
            }

            foreach (string s in deny)
            {
                if (s.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    retorno = "Não";
                }
            }
            return retorno;
        }
    }
}