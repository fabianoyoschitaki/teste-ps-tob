using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using PortoSeguroBOT.Bean;
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
    [Template(TemplateUsage.NotUnderstood, "Eu não entendo \"{0}\".", "Tente novamente.")]
    public class FormSeguroViagem
    {
        public IDialogContext ctx { get; set; }
        public FormSeguroViagem()
        {

        }
        public FormSeguroViagem (IDialogContext ctx)
        {
            this.ctx = ctx;
        }

        [Prompt("Qual o estado de origem da sua viagem?")]
        [Template(TemplateUsage.NotUnderstood, "Estado Inválido")]
        public string Origem;

        [Prompt("Qual o país de destino da sua viagem?")]
        [Template(TemplateUsage.NotUnderstood, "País Inválidox")]
        public string Destino;

        [Optional]
        [Prompt("Nesta viagem visitará também algum país europeu?")]
        [Template(TemplateUsage.NotUnderstood, "Não entendi a sua resposta")]
        public string PaisEuropeu;

        [Prompt("Qual a data da sua partida? Escreva no padrão dd/mm/aaaa")]
        [Template(TemplateUsage.NotUnderstood, "Eu não entendi \"{0}\".", "Tente novamente com o formato dd/mm/aaaa, eu não entendi \"{0}\".")]
        [Pattern(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$")]
        public DateTime DataPartida { get; set; }

        [Prompt("Qual a data do seu retorno? Escreva no padrão dd/mm/aaaa")]
        [Template(TemplateUsage.NotUnderstood, "Eu não entendi \"{0}\".", "Tente novamente com o formato dd/mm/aaaa, eu não entendi \"{0}\".")]
        [Pattern(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$")]
        public DateTime DataRetorno { get; set; }

        [Prompt("Quantos passageiros com menos de 70 anos (inclua o passageiro principal)?")]
        [Numeric(1, 10)]
        [Template(TemplateUsage.NotUnderstood, "Quantidade inválida")]
        public int Menor70;

        [Prompt("Quantos passageiros com mais de 70 anos (inclua o passageiro principal)?")]
        [Numeric(1, 10)]
        [Template(TemplateUsage.NotUnderstood, "Quantidade inválida")]
        public int Maior70;

        [Prompt("Essa viagem irá incluir a prática de algum esporte ou aventura?")]
        [Template(TemplateUsage.NotUnderstood, "Não entendi sua resposta")]
        public string ViagemAventura;

        [Prompt("Qual o {&} da sua viagem? {||}")]
        [Describe("motivo")]
        public MotivoViagemOptions? MotivoDaViagem;

        public static IForm<FormSeguroViagem> SeguroBuildForm()
        {
            //Callback para final de formulário
            OnCompletionAsyncDelegate<FormSeguroViagem> processandoCalculo = async (context, state) =>
            {
                context.UserData.SetValue("DadosSeguroViagem", PopulateContextWithData(state));
                await context.PostAsync($"Aguarde um momento enquanto calculamos seu seguro.");
            };

            return new FormBuilder<FormSeguroViagem>()
                .Message("Benvindo ao Seguro Viagem!")
                .Field(nameof(Origem), validate: ValidateUF)
                .Field(nameof(Destino), validate: ValidatePais)
                .Message("Você escolheu o estado {Origem} com destino a {Destino}.")
                //.Field(nameof(PaisEuropeu), validate: ValidateSimNao)
                //.Field(nameof(DataPartida), validate: ValidateStartDate)
                //.Field(nameof(DataRetorno), validate: ValidateEndDate)
                //.Field(nameof(Menor70), validate: ValidateQtd)
                //.Field(nameof(Maior70), validate: ValidateQtd)
                .Field(nameof(Menor70))
                .Confirm(async (state) =>
                {
                    var cost = 0.0;
                    switch (state.Menor70)
                    {
                        case 1: cost = 5.0; break;
                        case 2: cost = 6.50; break;
                    }
                    return new PromptAttribute($"Seu valor será {cost:C2}, tudo bem?");
                })
                .Field(nameof(Maior70))
                //.Field(nameof(ViagemAventura), validate: ValidateSimNao)
                .Field(nameof(MotivoDaViagem), validate: ValidateMotivoViagem)
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

        private static Task<ValidateResult> ValidateSimNao(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };

            if (ConstantsBase.getConfirmation(value.ToString()) == null)
            {
                result.Feedback = "Não entendi a sua resposta";
                result.IsValid = false;
            }
            else
            {
                result.Value = ConstantsBase.getConfirmation(value.ToString());
            }

            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateStartDate(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };
            var startDate = (DateTime)value;
            DateTime formattedDt;
            if (DateTime.TryParse(startDate.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                startDate = formattedDt;
            }
            if (startDate < DateTime.Today)
            {
                result.Feedback = "Data inicial inválida";
                result.IsValid = false;
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateEndDate(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };
            var startDate = state.DataPartida;
            DateTime formattedDt;
            if (DateTime.TryParse(state.DataPartida.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                startDate = formattedDt;
            }
            var endDate = (DateTime)value;
            if (DateTime.TryParse(
                endDate.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                endDate = formattedDt;
            }
            if (endDate < startDate)
            {
                result.Feedback = "Data final inválida";
                result.IsValid = false;
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateQtd(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };
            int newValue;
            if (!int.TryParse(value.ToString(), out newValue))
            {
                result.Feedback = "Não entendi a sua resposta";
                result.IsValid = false;
            }
            else
            {
                if(newValue < 0)
                {
                    result.Feedback = "Quantidade inválida";
                    result.IsValid = false;
                }
            }

            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateMotivoViagem(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };
        
            return Task.FromResult(result);
        }

        private static Boolean DestinoIsEurope(FormSeguroViagem state, object value)
        {
            return false;
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
                if (estado.Descricao.Equals(valor, StringComparison.InvariantCultureIgnoreCase))
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

        public static int GetCodMotivo(string valor)
        {
           switch(valor)
            {
                case "Lazer":
                    return 1;
                case "Negocios":
                    return 2;
                case "Estudos":
                    return 3;
                case "VisitaAmigoOuParente":
                    return 4;
                default:
                    return 0;
            }
        }

        private static SeguroViagem PopulateContextWithData(FormSeguroViagem state)
        {
            SeguroViagem seguro = new SeguroViagem();
            seguro.UfOrigem = GetEstado(state.Origem).Sigla;
            seguro.EstadoOrigem = state.Origem;
            seguro.PaisDestino = state.Destino;
            seguro.CodPaisDestino = GetPais(state.Destino).CodigoPais;
            seguro.CodContinente = GetPais(state.Destino).CodigoContinente;
            seguro.PaisEuropeuDestino = state.PaisEuropeu == "Sim" ? true : false;
            seguro.DataPartida = state.DataPartida;
            seguro.DataRetorno = state.DataRetorno;
            seguro.Menor70 = state.Menor70;
            seguro.Maior70 = state.Maior70;
            seguro.Motivo = state.MotivoDaViagem.ToString();
            seguro.CodMotivo = GetCodMotivo(state.MotivoDaViagem.ToString());
            return seguro;
        }

        public enum MotivoViagemOptions { Lazer, Negocios, Estudos, VisitaAmigoOuParente }
    }
}