using AppTailler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler.Pages
{
    public class RootPage : MasterDetailPage
    {
        #region Variaveis
        MenuPage menuPage;
        private usuarios usu;
        private clientes cli;
        private locais loc;
        #endregion
        public RootPage(usuarios user, clientes cliente, locais local, string pagina, int tela_atual)
        {
            var menu = this;
            usu = user;
            loc = local;
            cli = cliente;
            NavigationPage.SetHasNavigationBar(this, false);
            menuPage = new MenuPage(user, cli, loc, tela_atual);
            menuPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as MenuItem);
            Master = menuPage;
            if (pagina == "cliente")
            {
                Detail = new Cliente(user,menu);
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }
            if (pagina == "local")
            {
                Detail = new Local(user, cli,menu)
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }
            if (pagina == "auditoria")
            {
                Detail = new Auditoria(loc, cli, user,menu)
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }
            if (pagina == "relatorio")
            {
                Detail = new Relatorio(user, cli, loc, null)
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }
        }

        async void NavigateTo(MenuItem menu)
        {
            if (menu == null)
                return;

            //Page displayPage = (Page)Activator.CreateInstance(menu.TargetType);

            if (menu.TargetType == "cliente")
            {
                Detail = new Cliente(usu,this)
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }
            if (menu.TargetType == "local")
            {
                Detail = new Local(usu, cli, this)
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }
            if (menu.TargetType == "auditoria")
            {
                Detail = new Auditoria(loc, cli, usu,this)
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }

            if (menu.TargetType == "relatorio")
            {
                Detail = new Relatorio(usu, cli, loc, null)
                {
                    //BarBackgroundColor = Color.FromHex("#CDD1D7"),
                };
            }

            if (menu.TargetType == "cadastroresponsavel")
            {
                await App.NP.Navigation.PushAsync(new CadastroResponsavel(usu, cli, loc, null));
            }
            if (menu.TargetType == "ordemservico")
            {
                await App.NP.Navigation.PushAsync(new CadastroResponsavel(usu, cli, loc, null));
            }


            menuPage.Menu.SelectedItem = null;
            IsPresented = false;
        }
    }
}
