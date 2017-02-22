using Newtonsoft.Json;
using AppTailler.Models;
using AppTailler.Pages;
using AppTailler.ViewModels;
using AppTailler.Views;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Plugin.Connectivity;
using System.ComponentModel;

namespace AppTailler
{
    public partial class Relatorio : ContentPage
    {
        private usuarios usu;
        private clientes cl;
        private locais loc;
        private filaAuditoria _Fila = new filaAuditoria();
        public List<locais> clientesList;
        public byte[] m_Bytes;
        public byte[] array;
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        public Relatorio(usuarios user, clientes cli, locais l, byte[] bt)
        {
            BindingContext = this;
            InitializeComponent();
            //Ação do evento de clicar no botão menu lateral superior esquerdo
            var menuTut = new TapGestureRecognizer();
            menuTut.Tapped += delegate
            {
                App.MP.IsPresented = true;
            };
            menuLogo.GestureRecognizers.Add(menuTut);
            NavigationPage.SetHasNavigationBar(this, false);
            infosResp.Text = cli.RespNome + " | " + cli.RespTelefone + " | " + cli.RespEmail;
            usu = user;
            cl = cli;
            loc = l;
            m_Bytes = bt;
            GetAsync(null, usu);

            //Carregar infos
            if (!String.IsNullOrEmpty(cl.Classificacao))
            {
                if (cl.Classificacao == "A")
                {
                    good_image.Source = "muitosatisfeito_selecionado.png";
                    regular_image.Source = "satisfeito.png";
                    bad_image.Source = "poucosatisfeito.png";
                }
                else if (cl.Classificacao == "B")
                {
                    good_image.Source = "muitosatisfeito.png";
                    regular_image.Source = "satisfeitoselecionado.png";
                    bad_image.Source = "poucosatisfeito.png";
                }
                else if (cl.Classificacao == "C")
                {
                    good_image.Source = "muitosatisfeito.png";
                    regular_image.Source = "satisfeito.png";
                    bad_image.Source = "poucosatisfeitoselecionado.png";
                    cl.Classificacao = "C";
                }
            }
            inputEntry_nome.Text = cl.RespNome;
            inputEntry_cargo.Text = cl.RespCargo;
            inputEntry_telefone.Text = cl.RespTelefone;
            inputEntry_email.Text = cl.RespEmail;
            if (m_Bytes != null)
            {
                inputEntry_assinatura.Source = ImageSource.FromStream(() => new MemoryStream(m_Bytes));
            }
            else if (cl.AssinaturaArray != null)
            {
                inputEntry_assinatura.Source = ImageSource.FromStream(() => new MemoryStream(cl.AssinaturaArray));
            }
            button_assinatura.Clicked += async delegate
            {
                using (var dados = new DataAccess())
                {
                    cl.RespNome = inputEntry_nome.Text;
                    cl.RespCargo = inputEntry_cargo.Text;
                    cl.RespTelefone = inputEntry_telefone.Text;
                    cl.RespEmail = inputEntry_email.Text;
                    dados.AtualizarClientes(cl);
                }
                var navigationPage = new SignaturePadConfigView(this, usu, cl, loc, "fechamento");

                App.NP.Popped += (sender, args) =>
                {
                    (args.Page.BindingContext as BaseViewModel)?.OnDisappearing();
                };
                App.NP.Pushed += (sender, args) =>
                {
                    (args.Page.BindingContext as BaseViewModel)?.OnAppearing();
                };
                await App.NP.Navigation.PushAsync(navigationPage);
            };

            var good_imageTouched = new TapGestureRecognizer();
            good_imageTouched.Tapped += delegate
            {

                good_image.Source = "muitosatisfeito_selecionado.png";
                regular_image.Source = "satisfeito.png";
                bad_image.Source = "poucosatisfeito.png";
                cl.Classificacao = "A";

            };

            var regular_imageTouched = new TapGestureRecognizer();
            regular_imageTouched.Tapped += delegate
            {
                good_image.Source = "muitosatisfeito.png";
                regular_image.Source = "satisfeitoselecionado.png";
                bad_image.Source = "poucosatisfeito.png";
                cl.Classificacao = "B";
            };

            var bad_imageTouched = new TapGestureRecognizer();
            bad_imageTouched.Tapped += delegate
            {
                good_image.Source = "muitosatisfeito.png";
                regular_image.Source = "satisfeito.png";
                bad_image.Source = "poucosatisfeitoselecionado.png";
                cl.Classificacao = "C";
            };

            good_image.GestureRecognizers.Add(good_imageTouched);
            regular_image.GestureRecognizers.Add(regular_imageTouched);
            bad_image.GestureRecognizers.Add(bad_imageTouched);
        }

        public async void GetAsync(string tipo, usuarios user)
        {
            if (App.GLOBAL_SETS.modoOffline)
            {
                using (var dados = new DataAccess())
                {
                    clientesList = dados.GetAllLocais(cl.IdUnidade);
                }
            }
            else
            {
                var IdUnidade = cl.IdUnidade;
                var IdPessoa = cl.idPessoa;

                var idUsuario = user.idUsuario;
                using (var dados = new DataAccess())
                {
                    clientesList = dados.GetAllLocais(cl.IdUnidade);
                    generateGrid(clientesList);
                }
            }
        }

        private void generateGrid(List<locais> content)
        {
            int x = 0;
            int y = 0;
            foreach (locais item in content)
            {
                if (x == 4)
                {
                    y = y + 1;
                    x = 0;
                }
                var textLabel = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#384D79"),
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    Text = item.Nome,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                };

                var imageBg = new Image
                {
                    Source = "hex.png",
                    HeightRequest = 10,
                    HorizontalOptions = LayoutOptions.Center,
                };

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
                var stacklayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { imageBg, textLabel }
                };
                var lay_interno = new StackLayout
                {
                    Children = { stacklayout }
                };
                gridView.Children.Add(lay_interno, x, y);

                x = x + 1;
            };
        }

        public async void encerrarClicked(object sender, EventArgs e)
        {
            #region Validações de Campo
            String timeStamp = GetTimestamp(DateTime.Now);
            Atualizar atualizar = new Atualizar();
            if (!string.IsNullOrEmpty(cl.Classificacao) && !string.IsNullOrEmpty(inputEntry_nome.Text) && !string.IsNullOrEmpty(inputEntry_cargo.Text) && !string.IsNullOrEmpty(inputEntry_telefone.Text) && !string.IsNullOrEmpty(inputEntry_email.Text) && (m_Bytes != null || cl.AssinaturaArray != null))
            {
                using (var dados = new DataAccess())
                {
                    cl.RespNome = inputEntry_nome.Text;
                    cl.RespCargo = inputEntry_cargo.Text;
                    cl.RespTelefone = inputEntry_telefone.Text;
                    cl.RespEmail = inputEntry_email.Text;
                    dados.AtualizarClientes(cl);
                    var internet = CrossConnectivity.Current.IsConnected;
                    if (internet)
                    {
                        #region Envia Assinatura
                        if (m_Bytes != null)
                        {
                            array = m_Bytes;
                        }
                        else if (cl.AssinaturaArray != null)
                        {
                            array = cl.AssinaturaArray;
                        }

                        imagem assinatura = new imagem()
                        {
                            NomeImagem = timeStamp + ".png",
                            ArrayBytes = array,
                            Tipo = "AssinaturaFechamento"
                        };
                        cl.Assinatura = timeStamp + ".png";
                        cl.AssinaturaArray = array;
                        dados.AtualizarClientes(cl);
                        var result = await atualizar.EnviarImagem(assinatura);
                        if (result != "True")
                        {
                            await
                            DisplayAlert("Alerta",
                                "Houve um problema com o servidor de envio de imagens de assinatura, verifique sua conexão e tente novamente mais tarde!",
                                "Ok");
                            return;
                        }
                        #region Cria fila de envio
                        else
                        {
                            try
                            {
                                filaAuditoria Fila = dados.GetfilaAuditoria(cl.idPessoa);
                                if (Fila == null)
                                {
                                    Fila = new filaAuditoria();
                                    Fila.IdPessoa = cl.idPessoa;
                                    Fila.IdUnidade = cl.IdUnidade;
                                    //Fila.IdLocal = loc.IdLocal;
                                    Fila.listaLocal = Fila.AtualizarLocaisNaoConcluidos();
                                    Fila.Cargo = inputEntry_cargo.Text;
                                    Fila.Nome = inputEntry_nome.Text;
                                    Fila.Classificacao = cl.Classificacao;
                                    Fila.TotalFila = Fila.CountLocais();
                                    Fila.RestoFila = Fila.CountLocaisRestantes();
                                    dados.InserirFilaAuditoria(Fila);
                                    _Fila = Fila;
                                }
                                else
                                {
                                    var resposta = await DisplayAlert("Alerta", "Você já possui uma auditoria na fila de envio. O que deseja fazer?", "Enviar", "Deletar");
                                    if (resposta)
                                    {
                                        List<locais> lista = Fila.AtualizarLocaisNaoConcluidos();
                                        clientes c = new clientes();
                                        c = dados.GetClientes(Fila.IdUnidade);
                                        var resultado = await atualizar.EnviarAuditoriaFila(lista, c);
                                        if (resultado == "Sucesso")
                                        {
                                            dados.DeletefilaAuditoria(Fila);
                                        }
                                        _Fila = Fila;
                                    }
                                    else
                                    {
                                        dados.DeletefilaAuditoria(Fila);
                                        return;
                                    }
                                }
                            }
                            catch { }
                            string texto = _Fila.ListaLocaisSemAssinatura();
                            if (texto == "sucesso")
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {

                                    var resposta = await atualizar.EnviarAuditoriaFila(clientesList, cl);
                                    if (resposta != "Sucesso")
                                    {
                                        await DisplayAlert("Alerta", resposta, "Ok");
                                    }
                                    else
                                    {
                                        await DisplayAlert("Sucesso", "Auditorias enviadas com sucesso.", "Ok");
                                        try
                                        {
                                            dados.DeletefilaAuditoria(_Fila);
                                        }
                                        catch
                                        {

                                        }


                                    }

                                });
                            }
                            else
                            {
                                await DisplayAlert("Alerta", "Os seguintes locais não possuem assinatura vinculada: " + texto, "Vou verificar!");
                                dados.DeletefilaAuditoria(_Fila);
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Você não possui conexão com a internet, por favor conecte-se e tente novamente.", "Ok");
                    }
                }
                #endregion
                #endregion
            }
            else if (string.IsNullOrEmpty(cl.Classificacao))
            {
                await DisplayAlert("Alerta", "Por favor atribua uma classificação a auditoria.", "Ok");
            }
            else if (string.IsNullOrEmpty(inputEntry_nome.Text))
            {
                await DisplayAlert("Alerta", "Por favor preencha o campo de nome.", "Ok");
                inputEntry_nome.Focus();
            }
            else if (string.IsNullOrEmpty(inputEntry_cargo.Text))
            {
                await DisplayAlert("Alerta", "Por favor preencha o campo de cargo.", "Ok");
                inputEntry_cargo.Focus();
            }
            else if (string.IsNullOrEmpty(inputEntry_telefone.Text))
            {
                await DisplayAlert("Alerta", "Por favor preencha o campo de telefone.", "Ok");
                inputEntry_telefone.Focus();
            }
            else if (string.IsNullOrEmpty(inputEntry_email.Text))
            {
                await DisplayAlert("Alerta", "Por favor preencha o campo de email.", "Ok");
                inputEntry_email.Focus();
            }
            else if (m_Bytes == null && cl.AssinaturaArray == null)
            {
                await DisplayAlert("Alerta", "Por favor preencha o campo de assinatura.", "Ok");
            }
            #endregion
            //Carregando = false;
            //App.Carregando = false;
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

    }
}
