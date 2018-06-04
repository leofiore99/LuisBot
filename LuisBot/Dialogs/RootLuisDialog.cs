using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace LuisBot.Dialogs
{
    [LuisModel("cf355725-13f6-4b64-b7d6-33583d7919e5", "90a2e1e2de074cc49a93f7fa3c450099")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Desculpe, eu ainda não entendo sobre '{result.Query}'. Mas se quiser agendar uma viagem, é comigo mesmo!";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Cumprimento")]
        public async Task Cumprimento(IDialogContext context, LuisResult result)
        {
            string message = $"Olá! Que tal agendar um horário conosco? Talvez um corte, ou uma tintura!";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        //[LuisIntent("Viagem")]
        //public async Task Viagem(IDialogContext context, LuisResult result)
        //{
        //    GetEntityRecommendation("Localidade::De", "De", result);
        //    GetEntityRecommendation("Localidade::Para", "Para", result);
        //    GetEntityRecommendation("builtin.datetime.date", "Data", result);

        //    await context.PostAsync("Planejando sua viagem...");

        //    var form = new ViagemForm();
        //    var hotelsFormDialog = new FormDialog<ViagemForm>(form, this.BuildHotelsForm, FormOptions.PromptInStart, result.Entities);
        //    context.Call(hotelsFormDialog, this.ResumeAfterFlightFormDialog);
        //}

        //private IForm<ViagemForm> BuildHotelsForm()
        //{
        //    OnCompletionAsyncDelegate<ViagemForm> processBooking = async (context, state) =>
        //    {
        //        var message = "Obrigado pelo contato, estamos prestes a agendar sua viagem...";
        //        await context.PostAsync(message);
        //    };

        //    return new FormBuilder<ViagemForm>()
        //        .Field(nameof(ViagemForm.De), (state) => string.IsNullOrEmpty(state.De))
        //        .Field(nameof(ViagemForm.Para), (state) => string.IsNullOrEmpty(state.Para))
        //        .Field(nameof(ViagemForm.Data), (state) => string.IsNullOrEmpty(state.Data))
        //        .OnCompletion(processBooking)
        //        .Build();
        //}

        //private async Task ResumeAfterFlightFormDialog(IDialogContext context, IAwaitable<ViagemForm> result)
        //{
        //    try
        //    {
        //        var searchQuery = await result;

        //        var fromLocation = searchQuery.De;
        //        var toLocation = searchQuery.Para;
        //        var scheduledDate = searchQuery.Data;

        //        ////EXECUTE  BOOKFLIGHT

        //        var message = $"Sua viagem foi agendada para {scheduledDate} partindo de {fromLocation} até {toLocation}";
        //        await context.PostAsync(message);
        //    }
        //    catch (FormCanceledException ex)
        //    {
        //        string reply;

        //        if (ex.InnerException == null)
        //        {
        //            reply = "Você cancelou a operação.";
        //        }
        //        else
        //        {
        //            reply = $"Oops! Alguma de errado ocorreu :( Detalhes técnicos: {ex.InnerException.Message}";
        //        }

        //        await context.PostAsync(reply);
        //    }
        //    finally
        //    {
        //        context.Done<object>(null);
        //    }
        //}

        private static void GetEntityRecommendation(string fromType, string toType, LuisResult result)
        {
            EntityRecommendation dateEntityRecommendation;
            if (result.TryFindEntity(fromType, out dateEntityRecommendation))
            {
                dateEntityRecommendation.Type = toType;
            }

        }



        [LuisIntent("Agendamento")]
        public async Task Agendamento(IDialogContext context, LuisResult result)
        {
            GetEntityRecommendation("TipoAgendamento", "TipoAgendamento", result);
            GetEntityRecommendation("builtin.datetime.date", "Data", result);

            await context.PostAsync("Ok, vamos lá...");

            var form = new AgendamentoForm();
            var hotelsFormDialog = new FormDialog<AgendamentoForm>(form, this.BuildHairForm, FormOptions.PromptInStart, result.Entities);
            context.Call(hotelsFormDialog, this.ResumeAfterHairFormDialog);
        }

        private IForm<AgendamentoForm> BuildHairForm()
        {
            OnCompletionAsyncDelegate<AgendamentoForm> processBooking = async (context, state) =>
            {
                var message = "Obrigado pelo contato, estamos prestes a agendar seu horário...";
                await context.PostAsync(message);
            };

            return new FormBuilder<AgendamentoForm>()
                .Field(nameof(AgendamentoForm.TipoAgendamento), (state) => string.IsNullOrEmpty(state.TipoAgendamento))
                .Field(nameof(AgendamentoForm.Data), (state) => string.IsNullOrEmpty(state.Data))
                .OnCompletion(processBooking)
                .Build();
        }

        private async Task ResumeAfterHairFormDialog(IDialogContext context, IAwaitable<AgendamentoForm> result)
        {
            try
            {
                var searchQuery = await result;

                var tipoAgendamento = searchQuery.TipoAgendamento;
                var data = searchQuery.Data;

                ////EXECUTE  BOOKFLIGHT

                var message = $"Seu horário para realizar {tipoAgendamento} foi agendado para {data}! Estaremos lhe aguardando!";
                await context.PostAsync(message);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "Você cancelou a operação.";
                }
                else
                {
                    reply = $"Oops! Alguma de errado ocorreu :( Detalhes técnicos: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }




        [Serializable]
        public class ViagemForm
        {
            [Prompt("De qual cidade você pretende partir? {||}", AllowDefault = BoolDefault.True)]
            [Describe("Local, exemplo: São Paulo")]
            public string De { get; set; }

            [Prompt("Para qual cidade você quer viajar? {||}", AllowDefault = BoolDefault.True)]
            [Describe("Local, exemplo: Jundiaí")]
            public string Para { get; set; }

            [Prompt("Quando você quer viajar? {||}", AllowDefault = BoolDefault.True)]
            [Describe("Data, exemplo: amanhã, próxima semana ou qualquer data, como: 12-06-2018")]
            public string Data { get; set; }
        }

        [Serializable]
        public class AgendamentoForm
        {
            [Prompt("O que você gostaria de fazer no seu cabelo? {||}", AllowDefault = BoolDefault.True)]
            [Describe("Tipo, exemplo: tintura")]
            public string TipoAgendamento { get; set; }

            [Prompt("Para quando você gostaria de agendar? {||}", AllowDefault = BoolDefault.True)]
            [Describe("Data, exemplo: amanhã, próxima semana ou qualquer data, como: 12-06-2018")]
            public string Data { get; set; }
        }
    }
}