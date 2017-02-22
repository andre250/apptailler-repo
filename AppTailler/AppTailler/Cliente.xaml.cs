using AppTailler.Models;
using AppTailler.Pages;
using System.Collections.Generic;
using System.ComponentModel;
using Plugin.Connectivity;
using Xamarin.Forms;
using System.Linq;
using System;

namespace AppTailler
{
    public partial class Cliente : ContentPage, INotifyPropertyChanged
    {
        //Cria variaveis de sessão
        private usuarios usu;
        public List<clientes> clientesList;
        private RootPage menu;
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
        public Cliente(usuarios user, RootPage menu_param)
        {
            IsLoading = false;
            BindingContext = this;
            InitializeComponent();
            menu = menu_param;
            usu = user;
            NavigationPage.SetHasNavigationBar(this, false);
            //Ação do evento de clicar no botão menu lateral superior esquerdo
            var menuTut = new TapGestureRecognizer();
            menuTut.Tapped += async delegate
            {
                var sair = await DisplayAlert("Alerta", "Você deseja se desconectar desse usuario?", "Sim", "Não");
                if (sair)
                {
                    App.GLOBAL_SETS.isPersist = false;
                    using (var dados = new DataAccess())
                    {
                        dados.AtualizarSettings(App.GLOBAL_SETS);
                    }
                    await App.NP.PopToRootAsync();
                }
            };
            menuLogout.GestureRecognizers.Add(menuTut);

            var atualizarTut = new TapGestureRecognizer();
            atualizarTut.Tapped += async delegate
            {
                IsLoading = true;
                var internet = CrossConnectivity.Current.IsConnected;
                if (!internet)
                {
                    await DisplayAlert("Alerta", "Você precisa estar conectado a internet para atualizar.", "Ok");
                }
                else
                {
                    using (var dados = new DataAccess())
                    {
                        Atualizar atualizar = new Atualizar();
                        await atualizar.AtualizarMotivos(0);
                        var idPessoa = user.IdPessoa;
                        atualizar = new Atualizar();
                        var async = await atualizar.AtualizarUnidades(idPessoa);
                        if (async != "HTTP error")
                        {
                            await
                        DisplayAlert("Sucesso",
                            "Atualização concluída com sucesso!",
                            "Ok");
                            clientesList = dados.GetAllClientes(user.IdPessoa);
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
                clientesList = clientesList.OrderByDescending(o => o.StatusAuditoria.Equals(1) ? 2 : 1)
                                       .ThenBy(o => o.Unidade).ToList();
                generateGrid(clientesList);
                IsLoading = false;
            };
            botAtualizar.GestureRecognizers.Add(atualizarTut);
        }
        public async void GetAsync(string tipo, usuarios user)
        {
            IsLoading = true;
            using (var dados = new DataAccess())
            {
                var internet = CrossConnectivity.Current.IsConnected;
                clientesList = dados.GetAllClientes(user.IdPessoa);
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
                        Atualizar atualizar = new Atualizar();
                        await atualizar.AtualizarMotivos(0);
                        var idPessoa = user.IdPessoa;
                        atualizar = new Atualizar();
                        var async = await atualizar.AtualizarUnidades(idPessoa);
                        if (async != "HTTP error")
                        {
                            await
                        DisplayAlert("Sucesso",
                            "Atualização concluída com sucesso!",
                            "Ok");
                            clientesList = dados.GetAllClientes(user.IdPessoa);
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
            clientesList = clientesList.OrderByDescending(o => o.StatusAuditoria.Equals(1) ? 2 : 1)
                                       .ThenBy(o => o.Unidade).ToList();
            generateGrid(clientesList);
        }
        private void generateGrid(List<clientes> content)
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
                            clientesList = dados.GetAllClientes(usu.IdPessoa);
                            dados.Dispose();
                            gridView.Children.Clear();
                            generateGrid(clientesList);
                        }
                    }
                    else
                    {
                        using (var dados = new DataAccess())
                        {
                            clientesList = dados.GetFilteredClientes(usu.IdPessoa, search.Text);
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
                    clientesList = dados.GetAllClientes(usu.IdPessoa);
                    dados.Dispose();
                    gridView.Children.Clear();
                    generateGrid(clientesList);
                }
            };

            foreach (clientes item in content)
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
                    FontSize = 9,
                    Text = item.Unidade,
                    WidthRequest = 100,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                };

                var imageBg = new Image
                {
                    Source = "hex.png",
                    WidthRequest = 100,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Margin = 0
                };

                var btTut = new TapGestureRecognizer();
                btTut.Tapped += async delegate
                {
                    IsLoading = true;
                    App.GLOBAL_SETS.tempIdUnidade = item.IdUnidade;
                    await App.NP.Navigation.PushAsync(new RootPage(usu, item, null, "local", 2));
                    IsLoading = false;
                };

                imageBg.GestureRecognizers.Add(btTut);

                var imageStatus = new Image();

                if (item.StatusAuditoria == 1)
                {
                    imageStatus.Source = "incompleto.png";
                }

                if (item.StatusAuditoria == 2)
                {
                    imageStatus.Source = "concluido.png";
                }

                var absoluteLayout = new AbsoluteLayout();

                absoluteLayout.HorizontalOptions = LayoutOptions.Center;
                absoluteLayout.VerticalOptions = LayoutOptions.Fill;
                absoluteLayout.Margin = 0;
                absoluteLayout.Children.Add(imageBg, new Rectangle(.5, .5, 1, 1), AbsoluteLayoutFlags.All);
                absoluteLayout.Children.Add(textLabel, new Rectangle(.5, .5, .5, .8), AbsoluteLayoutFlags.All);
                absoluteLayout.Children.Add(imageStatus, new Rectangle(.2, .2, .2, .2), AbsoluteLayoutFlags.All);
                var lay_interno = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Fill,
                    Children = { absoluteLayout }
                };
                gridView.Children.Add(lay_interno, x, y);
                x = x + 1;
                IsLoading = false;
            };

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            using (var dados = new DataAccess())
            {
                clientesList = dados.GetAllClientes(usu.IdPessoa);
            }
            App.GLOBAL_SETS.tempIdUsuario = usu.IdPessoa;
            GetAsync(null, usu);
            /*using (var dados = new DataAccess())
            {
                var internet = CrossConnectivity.Current.IsConnected;
                if (internet)
                {
                    filaAuditoria Fila = new filaAuditoria();
                    Fila = dados.GetfilaAuditoria(1);
                    try
                    {
                        List<locais> lista = Fila.AtualizarLocaisNaoConcluidos();
                        if (lista.Count() != 0)
                        {
                            var resposta = await DisplayAlert("Alerta", "Você possui auditorias que ainda não foram enviadas, deseja envia-las agora?", "Sim", "Não");
                            if (resposta)
                            {
                                Atualizar atualizar = new Atualizar();
                                clientes c = new clientes();
                                c = dados.GetClientes(Fila.IdUnidade);
                                var resultado = await atualizar.EnviarAuditoriaFila(lista, c);
                                if (resultado == "Sucesso")
                                {
                                    dados.DeletefilaAuditoria(Fila);
                                }
                            }
                        }
                    }
                    catch
                    {

                    }

                }
            }*/
        }
    }
}
