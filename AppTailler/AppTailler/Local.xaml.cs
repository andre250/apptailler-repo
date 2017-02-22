using AppTailler.Models;
using System.Collections.Generic;
using Plugin.Connectivity;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using AppTailler.Pages;
using System;

namespace AppTailler
{
    public partial class Local : ContentPage
    {
        private usuarios usu;
        private clientes cl;
        public List<locais> clientesList;
        private RootPage menu;
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public Local(usuarios user, clientes cli, RootPage menu_param)
        {
            BindingContext = this;
            InitializeComponent();
            NavigationPage.SetTitleIcon(this, "logo_title.png");
            var version = DependencyService.Get<IInfoService>().AppVersionName;
            versionLabel.Text = "Versão: " + version;
            menu = menu_param;
            usu = user;
            cl = cli;
            //Ação do evento de clicar no botão menu lateral superior esquerdo
            var menuTut = new TapGestureRecognizer();
            menuTut.Tapped += delegate
            {
                menu.IsPresented = true;
            };

            menuLogo.GestureRecognizers.Add(menuTut);
            //Ação do evento de clicar no botão atualizar

            var atualizarTut = new TapGestureRecognizer();
            atualizarTut.Tapped += async delegate
            {
                ActivityIndicatorLayout.IsVisible = true;
                activ.IsRunning = true;
                var internet = CrossConnectivity.Current.IsConnected;
                if (!internet)
                {
                    await DisplayAlert("Alerta", "Você precisa estar conectado a internet para atualizar.", "Ok");
                }
                else
                {
                    using (var dados = new DataAccess())
                    {
                        var IdUnidade = cl.IdUnidade;
                        var IdPessoa = cl.idPessoa;
                        Atualizar atualizar = new Atualizar();
                        var async = await atualizar.AtualizarTudo(IdUnidade, IdPessoa, this);
                        dados.AtualizarStatusAuditoriaClientes(cl);
                        if (async != "HTTP error")
                        {
                            await
                            DisplayAlert("Sucesso",
                            "Atualização concluída com sucesso!",
                            "Ok");
                            clientesList = dados.GetAllLocais(cl.IdUnidade);
                            clientesList = clientesList.OrderByDescending(o => o.Cancelado.Equals(0))
                                       .ThenBy(o => o.StatusAuditoria.Equals(1) ? 2 : 1)
                                       .ThenBy(o => o.Nome)
                                       .ToList();
                            generateGrid(clientesList);
                        }
                        else
                        {
                            await
                            DisplayAlert("Alerta",
                            "Houve um problema com o servidor de atualização de auditoria, verifique sua conexão e tente novamente mais tarde!",
                            "Ok");
                            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                            return;
                        }
                    }
                }
                ActivityIndicatorLayout.IsVisible = false;
                activ.IsRunning = false;
            };
            botAtualizar.GestureRecognizers.Add(atualizarTut);
        }
        public async void GetAsync(string tipo, usuarios user)
        {
            ActivityIndicatorLayout.IsVisible = true;
            activ.IsRunning = true;

            using (var dados = new DataAccess())
            {
                var internet = CrossConnectivity.Current.IsConnected;
                clientesList = dados.GetAllLocais(cl.IdUnidade);
                if (!internet)
                {
                    if (clientesList.Count == 0)
                    {
                        await DisplayAlert("Alerta", "Você precisa carregar a lista de locais online ou não existem locais para serem auditados.", "Ok");
                        App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    }
                }
                else
                {
                    if (clientesList.Count == 0)
                    {
                        var IdUnidade = cl.IdUnidade;
                        var IdPessoa = cl.idPessoa;
                        Atualizar atualizar = new Atualizar();
                        var async = await atualizar.AtualizarTudo(IdUnidade, IdPessoa, this);
                        dados.AtualizarStatusAuditoriaClientes(cl);
                        if (async != "HTTP error")
                        {
                            await
                        DisplayAlert("Sucesso",
                            "Atualização concluída com sucesso!",
                            "Ok");
                            clientesList = dados.GetAllLocais(cl.IdUnidade);
                        }
                        else
                        {
                            await
                        DisplayAlert("Alerta",
                            "Houve um problema com o servidor de atualização de auditoria, verifique sua conexão e tente novamente mais tarde!",
                            "Ok");
                            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                            return;
                        }
                    }
                }
            }
            clientesList = clientesList.OrderByDescending(o => o.Cancelado.Equals(0))
                                       .ThenBy(o => o.StatusAuditoria.Equals(1) ? 2 : 1)
                                       .ThenBy(o => o.Nome)
                                       .ToList();
            generateGrid(clientesList);
        }
        private void generateGrid(List<locais> content)
        {
            gridView.Children.Clear();
            int x = 0;
            int y = 0;

            search.SearchCommand = new Command(() =>
            {
                if (string.IsNullOrEmpty(search.Text))
                {
                    using (var dados = new DataAccess())
                    {
                        clientesList = dados.GetAllLocais(cl.IdUnidade);
                        dados.Dispose();
                        gridView.Children.Clear();
                        generateGrid(clientesList);
                    }
                }
                else
                {
                    using (var dados = new DataAccess())
                    {
                        clientesList = dados.GetFilteredLocais(cl.IdUnidade, search.Text);
                        dados.Dispose();
                        gridView.Children.Clear();
                        generateGrid(clientesList);
                    }
                }

            });

            search.Focused += (sender, e) =>
            {
                using (var dados = new DataAccess())
                {
                    clientesList = dados.GetAllLocais(cl.IdUnidade);
                    dados.Dispose();
                    gridView.Children.Clear();
                    generateGrid(clientesList);
                }
            };

            foreach (locais item in content)
            {
                if (x == 3)
                {
                    y = y + 1;
                    x = 0;
                }
                var textLabel = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    //FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontSize = 9,
                    Text = item.Nome,
                    WidthRequest = 100,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };


                var imageBg = new Image
                {
                    Source = "hex.png",
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    WidthRequest = 100,
                };
                var btTut = new TapGestureRecognizer();
                btTut.Tapped += async delegate
                {
                    App.GLOBAL_SETS.tempIdLocal = item.IdLocal;
                    Page modal = new MenuAuditoria2(usu, cl, item, null, cl.idPessoa);
                    await App.NP.Navigation.PushAsync(modal);
                };

                imageBg.GestureRecognizers.Add(btTut);

                if (item.Cancelado == 1)
                {
                    imageBg.Source = "hextransparente.png";
                    imageBg.IsOpaque = true;
                    imageBg.IsEnabled = false;
                }

                var imageStatus = new Image
                {
                    VerticalOptions = LayoutOptions.Fill
                };

                if (item.StatusAuditoria == 1)
                {
                    imageStatus.Source = "incompleto.png";
                }
                if (item.StatusAuditoria == 2)
                {
                    imageStatus.Source = "concluido.png";
                }
                var absoluteLayout = new AbsoluteLayout();
                absoluteLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;
                absoluteLayout.Children.Add(imageBg, new Rectangle(.5, .5, 1, 1), AbsoluteLayoutFlags.All);
                absoluteLayout.Children.Add(textLabel, new Rectangle(.5, .5, .5, .8), AbsoluteLayoutFlags.All);
                absoluteLayout.Children.Add(imageStatus, new Rectangle(.2, .2, .2, .2), AbsoluteLayoutFlags.All);
                var lay_interno = new StackLayout
                {
                    Children = { absoluteLayout }
                };
                gridView.Children.Add(lay_interno, x, y);
                x = x + 1;
            }
            var nome_resp = "";
            if (cl.Administrador == null)
            {
                nome_resp = cl.Administrador;
            }
            if (cl.Gestor == null)
            {
                nome_resp = cl.Gestor;
            }

            footer_up_label.Text = "Administrador: " + nome_resp;
            footer_down_label.Text = cl.Unidade;
            ActivityIndicatorLayout.IsVisible = false;
            activ.IsRunning = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (var dados = new DataAccess())
            {
                cl = dados.GetClientes(cl.IdUnidade);
                clientesList = dados.GetAllLocais(cl.IdUnidade);
            }
            GetAsync(null, usu);
        }
    }
}