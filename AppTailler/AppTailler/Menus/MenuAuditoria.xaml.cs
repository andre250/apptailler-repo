using AppTailler.Models;
using AppTailler.Pages;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace AppTailler
{
    public partial class MenuAuditoria : ContentPage
    {
        private Cliente cli;
        private usuarios usu;
        public MenuAuditoria(usuarios user, Cliente cliente)
        {

            InitializeComponent();
            BackgroundColor = new Color(0, 0, 0, 0.5);
            cli = cliente;
            usu = user;

        }

        async void auditoriaClicked(object sender, EventArgs e)
        {
            //cli.GetAsync(null, usu);
            using (var usu = new DataAccess())
            {
                usuarios us = usu.GetUsuarios(App.GLOBAL_SETS.lastLogin_userId);
                cli.GetAsync(null, us);
                await App.NP.Navigation.PopModalAsync();
            }
        }

        async void monitoramentoClicked(object sender, EventArgs e)
        {
            cli.GetAsync(null, usu);
            await App.NP.Navigation.PopModalAsync();
        }

        async void franquiaClicked(object sender, EventArgs e)
        {
            cli.GetAsync(null, usu);
            await App.NP.Navigation.PopModalAsync();
        }

        async void closeClicked(object sender, EventArgs e)
        {
            await App.NP.Navigation.PopModalAsync();
        }
    }
}