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
        public string TempIda;
        public string TempVolta;
        public bool TempIdaValid;
        public bool TempVoltaValid;

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

        [Prompt("Confirma sua Data de Partida como sendo {TempIda}? {||}", ChoiceFormat = "{1}")]
        public SimNao? confirmaDataPartida;

        [Prompt("A Data inicial fornecida não é uma data válida, digite uma data inicial válida ou SAIR para cancelar a cotação")]
        [Template(TemplateUsage.NotUnderstood, "Eu não entendi \"{0}\".", "Tente novamente com o formato dd/mm/aaaa, eu não entendi \"{0}\".")]
        [Pattern(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$")]
        public DateTime DataPartidaErr { get; set; }

        [Prompt("Qual a data da sua partida? Escreva no padrão dd/mm/aaaa")]
        [Template(TemplateUsage.NotUnderstood, "Eu não entendi \"{0}\".", "Tente novamente com o formato dd/mm/aaaa, eu não entendi \"{0}\".")]
        [Pattern(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$")]
        public DateTime DataPartida { get; set; }

        [Prompt("Confirma sua Data de Retorno como sendo {TempVolta}? {||}", ChoiceFormat = "{1}")]
        public SimNao? confirmaDataRetorno;

        [Prompt("A Data final fornecida não é uma data válida, digite uma data final válida ou SAIR para cancelar a cotação")]
        [Template(TemplateUsage.NotUnderstood, "Eu não entendi \"{0}\".", "Tente novamente com o formato dd/mm/aaaa, eu não entendi \"{0}\".")]
        [Pattern(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$")]
        public DateTime DataRetornoErr { get; set; }

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

            data.TryGetValue("Periodo::Volta", out this.TempVolta);
            data.TryGetValue("Periodo::Ida", out this.TempIda);
            if(this.TempIda != null) this.TempIda = this.TempIda.Replace(" ", "");
            if (this.TempVolta != null)  this.TempVolta = this.TempVolta.Replace(" ", "");

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

            DateTime tempIda;
            if (DateTime.TryParse(this.TempIda, out tempIda))
            {
                this.TempIdaValid = ValidadeStartDateExcerpt(tempIda);
            }                
            else
            {
                this.TempIdaValid = false;
            }

            DateTime tempVolta;
            if (DateTime.TryParse(this.TempVolta, out tempVolta))
            {
                this.TempVoltaValid  = ValidateEndDateExcerpt(tempIda,tempVolta);
            }
            else
            {
                this.TempVoltaValid = false;
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
                .Field(nameof(TempIda), active: alwaysFalse) //Precisa Incluir esse Field para ter visibilidade
                .Field(nameof(TempVolta), active: alwaysFalse) //Precisa Incluir esse Field para ter visibilidade
                .Field(nameof(confirmaEstado), active: HasOrign)
                .Field(nameof(notFoundEstado), active: IsOrignValid, validate: ValidateUF)
                .Field(nameof(Origem),active:ConfirmedOrign,validate: ValidateUF)
                .Field(nameof(confirmaPais), active: HasDestiny)
                .Field(nameof(notFoundPais), active: IsDestinyValid, validate: ValidatePais)
                .Field(nameof(Destino), active: ConfirmedDestiny, validate: ValidatePais)
                .Field(nameof(PaisEuropeu),active: DestinoIsEurope)
                .Field(nameof(confirmaDataPartida), active: ValidDataPartida)
                .Field(nameof(DataPartidaErr), active: IsIdaValid, validate: ValidateStartDateErr)
                .Field(nameof(DataPartida), active:ConfirmedDataPartida, validate: ValidateStartDate)
                .Field(nameof(confirmaDataRetorno), active: ValidDataRetorno)
                .Field(nameof(DataRetornoErr), active: IsVoltaValid, validate: ValidateEndDateErr)
                .Field(nameof(DataRetorno), active: ConfirmedDataRetorno, validate: ValidateEndDate)
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

        private static bool ValidDataPartida(FormSeguroViagem state)
        {
            if (state.TempIdaValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ValidDataRetorno(FormSeguroViagem state)
        {
            if (state.TempVoltaValid)
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
                result.Feedback = "Desculpe, esse não é um estado brasileiro válido, digite um estado ou SAIR para cancelar a cotação.";
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
                result.Feedback = "Desculpe, esse não é um estado país válido, digite um país ou SAIR para cancelar a cotação.";
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

        private static bool IsIdaValid(FormSeguroViagem state)
        {
            if (state.TempIdaValid || state.TempIda == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsVoltaValid(FormSeguroViagem state)
        {
            if (state.TempVoltaValid || state.TempVolta == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool ConfirmedDataPartida(FormSeguroViagem state)
        {
            if ((state.confirmaDataPartida == SimNao.Não || !state.TempIdaValid))
            {
                return true;
            }
            else
            {
                //state.DataPartida = DateTime.ParseExact(state.TempIda, "dd/MM/yyyy",null);   
                return false;
            }
        }

        private static Task<ValidateResult> ValidateStartDateErr(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };
            bool valid = ValidadeStartDateExcerpt(value);

            if (!valid)
            {
                result.Feedback = "";
                result.IsValid = false;
            }
            else
            {
                state.DataPartida = (DateTime)value;
                state.TempIdaValid = true;
                if(state.TempVolta != null)
                {
                    state.TempVoltaValid = ValidateEndDateExcerpt(state.DataPartida, state.TempVolta);
                }
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
            bool valid = ValidadeStartDateExcerpt(value);

            if (!valid)
            {
                result.Feedback = "Data inicial inválida, digite uma data inicial válida ou SAIR para cancelar a cotação";
                result.IsValid = false;
            }
            else
            {
                state.DataPartida = (DateTime)value;
                if (state.TempVolta != null)
                {
                    state.TempVoltaValid = ValidateEndDateExcerpt(state.DataPartida, state.TempVolta);
                }
            }
            return Task.FromResult(result);
        }

        public static bool ValidadeStartDateExcerpt(object value)
        {

            DateTime startDate;
            try
            {
                startDate = (DateTime)value;
            }
            catch (InvalidCastException e)
            {
                if(!DateTime.TryParse(value.ToString(),out startDate))
                {
                    return false;
                }
            }
            DateTime formattedDt;
            if (DateTime.TryParse(startDate.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                startDate = formattedDt;
            }
            if (startDate < DateTime.Today)
            {
                return false;
            }
            return true;
        }

        private static bool ConfirmedDataRetorno(FormSeguroViagem state)
        {
            if ((state.confirmaDataRetorno == SimNao.Não || !state.TempVoltaValid))
            {
                return true;
            }
            else
            {
                //state.DataRetorno = DateTime.ParseExact(state.TempVolta, "dd/MM/yyyy", null); ;
                return false;
            }
        }

        private static Task<ValidateResult> ValidateEndDateErr(FormSeguroViagem state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };
            var startDate = state.DataPartida;
            bool valid = ValidateEndDateExcerpt(state.DataPartida, value);

            if (!valid)
            {
                result.Feedback = "";
                result.IsValid = false;
            }
            else
            {
                state.DataRetorno = (DateTime)value;
                state.TempVoltaValid = true;
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
            bool valid = ValidateEndDateExcerpt(state.DataPartida, value);
            
            if (!valid)
            {
                result.Feedback = "Data final inválida, digite uma data final válida ou SAIR para cancelar a cotação";
                result.IsValid = false;
            }
            else
            {
               state.DataRetorno = (DateTime)value;
            }
            return Task.FromResult(result);
        }

        private static bool ValidateEndDateExcerpt(DateTime start, object value)
        {
            var startDate = start;
            DateTime formattedDt;
            if (DateTime.TryParse(start.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                startDate = formattedDt;
            }
            DateTime endDate;
            try
            {
                endDate = (DateTime)value;
            }
            catch (InvalidCastException e)
            {
                if (!DateTime.TryParse(value.ToString(), out endDate))
                {
                    return false;
                }
            }

            if (DateTime.TryParse(
                endDate.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                endDate = formattedDt;
            }
            if (endDate < startDate)
            {
                return false;
            }
            return true;
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
                result.Feedback = "Não entendi a sua resposta, digite uma quantidade válida ou SAIR para cancelar a cotação";
                result.IsValid = false;
            }
            else
            {
                if(newValue < 0)
                {
                    result.Feedback = "Quantidade inválida, digite uma quantidade válida ou SAIR para cancelar a cotação";
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
            if (valor != null)
            {
                foreach (Pais pais in ConstantsPais.PAISES)
                {
                    // TODO fazer tratamento para mais de um país, mostrar as opções de match
                    //if (Formatters.RemoveAcentos(pais.Descricao.ToLower()).Contains(Formatters.RemoveAcentos(valor.ToLower())))
                    if (Formatters.RemoveAcentos(pais.Descricao.ToLower()).Equals(Formatters.RemoveAcentos(valor.ToLower())))
                    {
                        retorno = pais;
                        break;
                    }

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