using AppTailler.Models;
using System;
using System.Diagnostics;
using AppTailler.Pages;
using Xamarin.Forms;
using Android.OS;

namespace AppTailler
{
    public partial class CancelamentoAuditoria : ContentPage
    {
        private locais Local;
        private clientes cl;
        private usuarios us;
        private int idPessoa;
        public CancelamentoAuditoria(usuarios user, clientes cli, locais loc, int IdPessoa)
        {
            InitializeComponent();
            Local = loc;
            cl = cli;
            us = user;
            idPessoa = IdPessoa;
            BackgroundColor = new Color(0, 0, 0, 0.5);
        }

        async void closeClicked(object sender, EventArgs e)
        {
            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
        }

        async void okClicked(object sender, EventArgs e)
        {
            var alerta = await DisplayAlert("Alerta", "Você deseja cancelar a auditoria em " + Local.Nome + "?", "Sim", "Não");
            if (alerta)
            {
                using (var dados = new DataAccess())
                {
                    Atualizar atualizar = new Atualizar();
                    vistoria vistoria = new vistoria
                    {
                        IdAuditoriaBase = Local.IdAuditoriaBase,
                        Evento = Local.cevento,
                        IdEvento = Local.IdEvento,
                        TipoVistoria= Local.TipoVistoria,
                        LocalCodigo = Local.IdLocal,
                        UnidadeCodigo = Local.IdUnidade,
                        Auditor = idPessoa,
                        Data = GetTimestamp(DateTime.Now, "dia"),
                        HoraInicio = GetTimestamp(DateTime.Now, "hora"),
                        HoraFim = GetTimestamp(DateTime.Now, "hora"),
                        RespNome = Local.RespNome,
                        RespTelefone = Local.RespTelefone,
                        RespEmail = Local.RespEmail,
                        Assinatura = Local.Assinatura,
                        Cancelado = 1,
                        MotivoCancelado = motivo.Text,
                        Norma = Local.IdNorma,
                        Versao = DependencyService.Get<IInfoService>().AppVersionName,
                        DeviceID = Build.Serial
                    };
                    csErro response = new csErro();
                    response = await atualizar.EnviarAuditoria(vistoria);
                    if (response.TeveErro)
                    {
                        await DisplayAlert("Alerta", response.Mensagem, "Ok");
                    }
                    else
                    {
                        Local.Cancelado = 1;
                        Local.StatusAuditoria = 0;
                        dados.AtualizarLocais(Local);
                        await DisplayAlert("Alerta", "Auditoria cancelada com sucesso", "Ok");
                        await App.NP.Navigation.PushAsync(new RootPage(us, cl, null, "local", 2));
                    }
                }
            }
        }

        #region GetTimestamp
        public String GetTimestamp(DateTime value, string modo)
        {
            if (modo == "dia")
            {
                return value.ToString("yyyy-MM-dd");
            }
            else if (modo == "hora")
            {
                return value.ToString("HH:mm:ss");
            }
            return "";

        }
        #endregion
    }
}