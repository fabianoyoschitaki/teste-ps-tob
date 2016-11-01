using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using PortoSeguroBOT.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static PortoSeguroBOT.Helpers.ConstantsPais;
using static PortoSeguroBOT.Helpers.ConstantsUF;

namespace PortoSeguroBOT.Form_Flows
{
    [Serializable]
    public class FormSeguroViagem
    {
        [Prompt("Qual o estado de origem da sua viagem? {||}")]
        [Template(TemplateUsage.NotUnderstood, "Estado Inválidox")]
        public string Origem;

        [Prompt("Qual o país de destino da sua viagem? {||}")]
        [Template(TemplateUsage.NotUnderstood, "País Inválidox")]
        public string Destino;

        public static IForm<FormSeguroViagem> SeguroBuildForm()
        {
            //Callback para final de formulário
            OnCompletionAsyncDelegate<FormSeguroViagem> processandoCalculo = async (context, state) =>
            {
                //await context.PostAsync($"Ok. Calculando seguro para {state.Destino} de {state.Origem} indo no dia {state.DataPartida} e voltando no dia {state.DataRetorno}");
                await context.PostAsync($"Fim de formulário, recebi {state.Origem} e {state.Destino}");
            };

            return new FormBuilder<FormSeguroViagem>()
                .Field(nameof(Origem), validate: ValidateUF)
                .Field(nameof(Destino), validate: ValidatePais)
                .OnCompletion(processandoCalculo)
                .Build();
        }

        private static Task<ValidateResult> ValidateUF(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };

            Estado estado = GetEstado(value.ToString());
            if (estado == null)
            {
                result.Feedback = "Estado Inválido";
                result.IsValid = false;
            }
            else
            {
                result.Value = estado.Descricao;
                ctx
            }

            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidatePais(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };

            Pais pais = GetPais(value.ToString());
            if (pais == null)
            {
                result.Feedback = "Pais Inválido";
                result.IsValid = false;
            }
            else
            {
                result.Value = pais.Descricao;
            }

            return Task.FromResult(result);
        }


        public static Estado GetEstado(string valor)
        {
            Estado retorno = null;
            foreach (Estado estado in ConstantsUF.ESTADOS)
            {
                if (estado.Sigla.Equals(valor, StringComparison.InvariantCultureIgnoreCase))
                {
                    retorno = estado;
                    break;
                }
            }
            return retorno;
        }

        public static Pais GetPais(string valor)
        {
            Pais retorno = null;
            foreach (Pais pais in ConstantsPais.PAISES)
            {
                if (pais.Descricao.Equals(valor, StringComparison.InvariantCultureIgnoreCase))
                {
                    retorno = pais;
                    break;
                }
            }
            return retorno;
        }
    }
}