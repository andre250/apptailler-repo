using AppTailler.Models;
using AppTailler.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppTailler.Menus;
using Plugin.Connectivity;
using Xamarin.Forms;
using Android.OS;

namespace AppTailler
{
    public class MenuPage : ContentPage
    {
        #region Declaração de variaveis
        public ListView Menu { get; set; }
        public auditoria auditoria;
        public List<areas_sql> listareas;
        public List<areas> listareasinsert = new List<areas>();
        public List<itens_sql> listitens;
        public List<subitens> listsubitens;
        public List<itens> listitensinsert = new List<itens>();
        public List<subitens> listsubitensinsert = new List<subitens>();
        #endregion
        public MenuPage(usuarios user, clientes cli, locais loc, int tela_atual)
        {
            Icon = "settings.png";
            Title = "menu";
            BackgroundColor = Color.FromHex("#CDD1D7");
            NavigationPage.SetHasNavigationBar(this, false);
            Menu = new MenuListView(tela_atual);
            string descText = "Não cadastrado";
            if (user != null && cli == null && loc == null)
            {
                descText = user.Nome;
            }
            else if (cli != null && loc == null)
            {
                descText = user.Nome + "\n" + cli.Unidade;
            }
            else if (user != null && cli != null && loc != null)
            {
                descText = user.Nome + "\n" + cli.Unidade + "\n" + loc.Nome;
            }


            var menuLabel = new ContentView
            {
                Padding = new Thickness(10, 10, 0, 5),

                Content = new Label
                {
                    TextColor = Color.FromHex("#FFFFFF"),
                    Text = descText,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                }

            };

            var botFinalizar = new Button
            {
                BackgroundColor = Color.FromHex("#CDD1D7"),
                Text = "Sincronizar",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = false
            };
            if (tela_atual == 3 || tela_atual == 4 || tela_atual == 5)
            {
                botFinalizar.IsEnabled = true;
            }

            botFinalizar.Clicked += async delegate
            {
                var internet = CrossConnectivity.Current.IsConnected;
                if (internet)
                {
                    if (loc.Assinatura != null)
                    {
                        Atualizar atualizar = new Atualizar();
                        
                        imagem assinatura = new imagem()
                        {
                            NomeImagem = loc.Assinatura,
                            ArrayBytes = loc.AssinaturaArray,
                            Tipo = "AssinaturaLocal"
                        };
                        await App.NP.Navigation.PushAsync(new Loading("Estamos enviando sua auditoria. Aguarde..."));
                        var result = await atualizar.EnviarImagem(assinatura);
                        if (result == "True")                        {
                            //await DisplayAlert("Sucesso", "Assinatura enviada com sucesso!", "Ok");
                        }
                        else
                        {
                            await
                                DisplayAlert("Falha",
                                    "Houve um erro com o servidor assinatura não foi enviada corretamente.", "Ok");
                            return;
                        }
                        using (var dados = new DataAccess())
                        {
                            //Lembrar de impossibilitar clicar nesse botao nas outras telas.
                            vistoria_temp temp = dados.GetVistoriaTemp(loc.IdLocal);
                            vistoria vistoria = new vistoria
                            {
                                LocalCodigo = temp.LocalCodigo,
                                UnidadeCodigo = temp.UnidadeCodigo,
                                Auditor = temp.Auditor,
                                Data = temp.Data,
                                HoraInicio = temp.HoraInicio,
                                HoraFim = temp.HoraFim,
                                RespNome = loc.RespNome,
                                RespTelefone = loc.RespTelefone,
                                RespEmail = loc.RespEmail,
                                RespCargo = loc.RespCargo,
                                Assinatura = loc.Assinatura,
                                IdAuditoriaBase = loc.IdAuditoriaBase,
                                Norma = loc.IdNorma,
                                TipoVistoria = loc.TipoVistoria,
                                IdEvento = cli.IdEvento,
                                Evento = cli.cevento,
                                Versao = DependencyService.Get<IInfoService>().AppVersionName,
                                DeviceID = Build.Serial
                        };
                            var listaImagens = vistoria.GetAreas(vistoria.LocalCodigo);
                            foreach (var img in listaImagens)
                            {
                                var fotoEnviada = await atualizar.EnviarImagem(img);
                                if (fotoEnviada != "True")
                                {
                                    await
                                    DisplayAlert("Alerta",
                                        "Houve um problema com o servidor de envio de fotos, verifique sua conexão e tente novamente mais tarde!",
                                        "Ok");
                                    return;
                                }
                            }
                            csErro response = new csErro();
                            response = await atualizar.EnviarAuditoria(vistoria);
                                                  
                            if (!response.TeveErro)
                            {
                                loc.StatusAuditoria = 2;
                                loc.IdAuditoria = loc.IdLocal;
                                dados.AtualizarLocais(loc);
                                bool allLocaisConcluidos = true;
                                List<locais> list = dados.GetAllLocais(cli.IdUnidade);
                                foreach (locais local in list)
                                {
                                    if (local.StatusAuditoria == 1 || local.StatusAuditoria == 0)
                                    {
                                        allLocaisConcluidos = false;
                                    }
                                }
                                if (allLocaisConcluidos)
                                {
                                    cli.StatusAuditoria = 2;
                                    dados.AtualizarClientes(cli);
                                }
                                Page modal = new AuditoriaFinalizada();
                                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                                await App.NP.Navigation.PushAsync(modal);
                                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 3]);
                                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 2]);
                                await Task.Delay(2000);
                                await App.NP.Navigation.PopAsync();
                            }
                            else
                            {
                                await
                                    DisplayAlert("Alerta",
                                        response.Mensagem,
                                        "Ok");
                                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "O responsável pelo local ainda não assinou.", "Vou verificar.");
                        Page modal = new CadastroResponsavel(user, cli, loc, null);
                        await App.NP.Navigation.PushAsync(modal);
                    }
                }
                else
                {
                    await DisplayAlert("Alerta", "Você não possui conexão, conecte-se e tente novamente.", "Ok");
                }
            };

            var botEmail = new Button
            {
                BackgroundColor = Color.FromHex("#CDD1D7"),
                Text = "Enviar Email",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = false
            };
            if (tela_atual == 3 || tela_atual == 4 || tela_atual == 5)
            {
                botEmail.IsEnabled = false;
            }
            botEmail.Clicked += async delegate
            {
                await App.NP.Navigation.PushAsync(new RootPage(user, cli, loc, "relatorio", 4));
            };

            var btTut = new TapGestureRecognizer();
            btTut.Tapped += async delegate
            {
                var sair = await DisplayAlert("Alerta", "Você deseja sair?", "Sim", "Não");
                if (sair)
                {
                    App.GLOBAL_SETS.isPersist = false;
                    using (var dados = new DataAccess())
                    {
                        dados.AtualizarSettings(App.GLOBAL_SETS);
                    }
                    Page modal = new Login();
                    await App.NP.Navigation.PushAsync(modal);
                }

            };

            var botLogout = new Image
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Source = "logout.png",
            };

            botLogout.GestureRecognizers.Add(btTut);

            var labelLogout = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                Text = "Sair"
            };

            var lay_sair = new StackLayout
            {
                BackgroundColor = Color.FromHex("#6E6E6F"),
                Orientation = StackOrientation.Horizontal,
                Children = { botLogout, labelLogout }
            };

            var lay_footer_menu = new StackLayout
            {
                BackgroundColor = Color.FromHex("#6E6E6F"),
                Orientation = StackOrientation.Horizontal,
                Children = { lay_sair }
            };

            var boxBots = new StackLayout
            {
                Padding = new Thickness(10, 20, 0, 70),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand

            };
            var version = DependencyService.Get<IInfoService>().AppVersionName;

            var labelVersion = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Versão: "+version
            };

            boxBots.Children.Add(botFinalizar);
            boxBots.Children.Add(labelVersion);

            var layboxBots = new StackLayout
            {
                BackgroundColor = Color.FromHex("#6E6E6F"),
                Padding = new Thickness(20, 0, 0, 20),
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    boxBots,
                    lay_footer_menu

                }
            };

            var listviewLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill
            };

            listviewLayout.Children.Add(Menu);

            var layout = new StackLayout
            {
                Spacing = 10,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#5B5B5E")

            };
            layout.Children.Add(menuLabel);
            layout.Children.Add(listviewLayout);
            layout.Children.Add(layboxBots);

            Content = layout;

        }


    }
}
