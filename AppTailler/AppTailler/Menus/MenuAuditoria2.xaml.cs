using AppTailler.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using AppTailler.Menus;
using AppTailler.Pages;
using Xamarin.Forms;

namespace AppTailler
{
    public partial class MenuAuditoria2 : ContentPage, INotifyPropertyChanged
    {
        private Auditoria aud;
        private clientes cl;
        private usuarios us;
        private locais loc;
        private int IdPessoa;
        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        public MenuAuditoria2(usuarios user, clientes cli, locais local, Auditoria auditoria, int idPessoa)
        {
            IsLoading = false;
            BindingContext = this;
            InitializeComponent();
            BackgroundColor = new Color(0, 0, 0, 0.5);
            IdPessoa = idPessoa;
            aud = auditoria;
            loc = local;
            cl = cli;
            us = user;
        }
        async void realizarauditoriaClicked(object sender, EventArgs e)
        {
            IsLoading = true;
            using (var dados = new DataAccess())
            {
                vistoria_temp temp = dados.GetVistoriaTemp(loc.IdLocal);
                if (temp != null)
                {
                    if (string.IsNullOrEmpty(temp.Data))
                    {
                        temp.Data = GetTimestamp(DateTime.Now, "dia");
                        dados.InserirAtualizarVistoriaTemp(temp);
                    }
                    if (string.IsNullOrEmpty(temp.HoraInicio))
                    {
                        temp.HoraInicio = GetTimestamp(DateTime.Now, "hora");
                        dados.InserirAtualizarVistoriaTemp(temp);
                    }
                }
                await App.NP.Navigation.PushAsync(new RootPage(us, cl, loc, "auditoria", 3));
            }
            IsLoading = false;
        }
        async void closeClicked(object sender, EventArgs e)
        {
            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
        }
        async void cancelarClicked(object sender, EventArgs e)
        {
            Page modal = new CancelamentoAuditoria(us, cl, loc, IdPessoa);
            await App.NP.Navigation.PushAsync(modal);
        }

        async void alterarCadastroClicked(object sender, EventArgs e)
        {
            Page modal = new CadastroResponsavel(us, cl, loc, null);
            await App.NP.Navigation.PushAsync(modal);
        }

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
            else if (modo == "diahora")
            {
                return value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return "";

        }


    }
}