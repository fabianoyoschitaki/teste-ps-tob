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
    public class FormSeguroViagem
    {
        public Dictionary<string, string> data { get; set; }
        public string TempOrigem;
        public string TempDestino;
        public bool TempOrigemValid;
        public bool TempDestinoValid;

        [Prompt("Confirma seu local de saída sendo: {TempOrigem}? {||}", ChoiceFormat = "{1}")]
        public SimNao? confirmaEstado;

        [Prompt("O local {TempOrigem} não é um estado válido, qual o estado de origem da sua viagem?")]
        public string notFoundEstado;

        [Prompt("Qual o estado de origem da sua viagem?")]
        [Template(TemplateUsage.NotUnderstood, "Estado Inválido")]
        public string Origem;

        [Prompt("Confirma seu local de destino sendo: {TempDestino}? {||}", ChoiceFormat = "{1}")]
        public SimNao? confirmaPais;

        [Prompt("O local {TempDestino} não é um país válido, qual o país de destino da sua viagem?")]
        public string notFoundPais;

        [Prompt("Qual o país de destino da sua viagem?")]
        [Template(TemplateUsage.NotUnderstood, "País Inválido")]
        public string Destino;

        [Prompt("Nesta viagem visitará também algum país europeu?{||}", ChoiceFormat = "{1}")]
        public SimNao? PaisEuropeu;

        [Prompt("Qual a data da sua partida? Escreva no padrão dd/mm/aaaa")]
        [Template(TemplateUsage.NotUnderstood, "Eu não entendi \"{0}\".", "Tente novamente com o formato dd/mm/aaaa, eu não entendi \"{0}\".")]
        [Pattern(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$")]
        public DateTime DataPartida { get; set; }

        [Prompt("Qual a data do seu retorno? Escreva no padrão dd/mm/aaaa")]
        [Template(TemplateUsage.NotUnderstood, "Eu não entendi \"{0}\".", "Tente novamente com o formato dd/mm/aaaa, eu não entendi \"{0}\".")]
        [Pattern(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$")]
        public DateTime DataRetorno { get; set; }

        [Prompt("Quantos passageiros com menos de 70 anos (inclua o passageiro principal)? {||}")]
        [Template(TemplateUsage.NotUnderstood, "Quantidade inválida")]
        public string Menor70;

        [Prompt("Quantos passageiros com mais de 70 anos (inclua o passageiro principal)? {||}")]
        [Template(TemplateUsage.NotUnderstood, "Quantidade inválida")]
        public string Maior70;

        [Prompt("Essa viagem irá incluir a prática de algum esporte ou aventura? {||}", ChoiceFormat="{1}")]
        public SimNao? ViagemAventura;

        [Prompt("Qual o motivo da sua viagem? {||}")]
        public MotivoViagemOptions? MotivoDaViagem;

        public FormSeguroViagem()
        {

        }

        public FormSeguroViagem(Dictionary<string, string> data)
        {
            this.data = data;
            data.TryGetValue("Localidade::LocalidadeOrigem", out this.TempOrigem);
            data.TryGetValue("Localidade::LocalidadeDestino", out this.TempDestino);

            if (GetEstado(this.TempOrigem) == null)
            {
                this.TempOrigemValid = false;
            }
            else
            {
                this.TempOrigem = GetEstado(this.TempOrigem).Descricao;
                this.TempOrigemValid = true;
            }

            if (GetPais(this.TempDestino) == null)
            {
                this.TempDestinoValid = false;
            }
            else
            {
                this.TempDestino = GetPais(this.TempDestino).Descricao;
                this.TempDestinoValid = true;
            }
        }

        public static IForm<FormSeguroViagem> SeguroBuildForm()
        {
            //Callback para final de formulário
            OnCompletionAsyncDelegate<FormSeguroViagem> processandoCalculo = async (context, state) =>
            {
                context.UserData.SetValue("DadosSeguroViagem", PopulateContextWithData(state));
                await context.PostAsync($"Aguarde um momento enquanto calculamos seu seguro.");
            };

            return FormUtils.CreateCustomForm<FormSeguroViagem>()
                .Field(nameof(TempOrigem), active: alwaysFalse) //Precisa Incluir esse Field para ter visibilidade
                .Field(nameof(TempDestino), active: alwaysFalse) //Precisa Incluir esse Field para ter visibilidade
                .Field(nameof(confirmaEstado), active: HasOrign)
                .Field(nameof(notFoundEstado), active: IsOrignValid, validate: ValidateUF)
                .Field(nameof(Origem),active:ConfirmedOrign,validate: ValidateUF)
                .Field(nameof(confirmaPais), active: HasDestiny)
                .Field(nameof(notFoundPais), active: IsDestinyValid, validate: ValidatePais)
                .Field(nameof(Destino), active: ConfirmedDestiny, validate: ValidatePais)
                .Field(nameof(PaisEuropeu),active: DestinoIsEurope)
                .Field(nameof(DataPartida), validate: ValidateStartDate)
                .Field(nameof(DataRetorno), validate: ValidateEndDate)
                .Field(nameof(Menor70), validate: ValidateQtd)
                .Field(nameof(Maior70), validate: ValidateQtd)
                .Field(nameof(ViagemAventura))
                .Field(nameof(MotivoDaViagem), validate: ValidateMotivoViagem)
                .OnCompletion(processandoCalculo)
                .Build();
        }

        private static bool alwaysFalse(FormSeguroViagem state)
        {
            return false;
       }
        private static bool HasOrign(FormSeguroViagem state)
        {
            if(state.TempOrigem != null && state.TempOrigemValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsOrignValid(FormSeguroViagem state)
        {
            if (state.TempOrigemValid || state.TempOrigem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool ConfirmedOrign(FormSeguroViagem state)
        {
            if (state.confirmaEstado == SimNao.Não || !state.TempOrigemValid)
            {
                return true;
            }
            else
            {
                state.Origem = state.TempOrigem;
                return false;
            }
        }

        private static bool HasDestiny(FormSeguroViagem state)
        {
            if (state.TempDestino != null && state.TempDestinoValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsDestinyValid(FormSeguroViagem state)
        {
            if (state.TempDestinoValid || state.TempDestino == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        private static bool ConfirmedDestiny(FormSeguroViagem state)
        {
            if (state.confirmaPais == SimNao.Não || !state.TempDestinoValid)
            {
                return true;
            }
            else
            {
                state.Destino = state.TempDestino;
                return false;
            }
        }

        private static bool DestinoIsEurope(FormSeguroViagem state) {
            Pais p = GetPais(state.Destino);
            bool ret;
            if(p != null)
            {
                ret = p.CodigoContinente == 3 ? false : true;
                return ret;
            }
            else
            {
                return false;
            }
           
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
                result.Feedback = "Desculpe, esse não é um estado brasileiro válido, digite um estado ou SAIR para cancelar a contação.";
                result.IsValid = false;
            }
            else
            {
                result.Value = estado.Descricao;
                state.Origem = estado.Descricao;
                state.confirmaEstado = SimNao.Sim;
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
                state.Destino = pais.Descricao;
                state.confirmaPais = SimNao.Sim;
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

            if ("nenhum".Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                value = 0;
                result.Value = "0";
            }

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
            seguro.PaisEuropeuDestino = state.PaisEuropeu == SimNao.Sim ? true : false;
            seguro.DataPartida = state.DataPartida;
            seguro.DataRetorno = state.DataRetorno;
            seguro.Menor70 = int.Parse(state.Menor70);
            seguro.Maior70 = int.Parse(state.Maior70);
            seguro.Motivo = state.MotivoDaViagem.ToString();
            seguro.CodMotivo = GetCodMotivo(state.MotivoDaViagem.ToString());
            return seguro;
        }

        public enum SimNao { Sim, Não }
        public enum MotivoViagemOptions { Lazer, Negocios, Estudos, VisitaAmigoOuParente }
    }
}