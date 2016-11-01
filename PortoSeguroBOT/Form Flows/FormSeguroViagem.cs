using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PortoSeguroBOT.Form_Flows
{
    [Serializable]
    public class FormSeguroViagem
    {
        [Prompt("Qual o estado de origem da sua viagem? {||}")]
        [Template(TemplateUsage.NotUnderstood, "Estado Inválido")]
        public string Origem;

        public static IForm<FormSeguroViagem> SeguroBuildForm()
        {
            //Callback para final de formulário
            OnCompletionAsyncDelegate<FormSeguroViagem> processandoCalculo = async (context, state) =>
            {
                //await context.PostAsync($"Ok. Calculando seguro para {state.Destino} de {state.Origem} indo no dia {state.DataPartida} e voltando no dia {state.DataRetorno}");
                await context.PostAsync($"Fim de formulário, recebi {state.Origem}");
            };

            return new FormBuilder<FormSeguroViagem>()
                .Field(nameof(Origem), validate: ValidateUF)
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

            if (!"SP".Equals(value))
            {
                result.Feedback = "Estado Inválido";
                result.IsValid = false;
            }
            else
            {
                result.Value = "NOVO VALOR";
            }

            return Task.FromResult(result);
        }

        public enum EstadoOrigem {SP, RJ, ES, MG, PE, SC, RS}
    }
}