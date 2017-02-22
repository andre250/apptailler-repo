
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AppTailler.Models;
using AppTailler.ViewModels;
using AppTailler.Views;
using Xamarin.Forms;
using Plugin.Connectivity;

namespace AppTailler
{
    public partial class CadastroResponsavel : ContentPage
    {
        private locais loc;
        private usuarios usu;
        private clientes cl;
        public byte[] m_Bytes;
        public CadastroResponsavel(usuarios user, clientes cli, locais local, byte[] bt)
        {
            InitializeComponent();
            BackgroundColor = new Color(0, 0, 0, 0.5);
            loc = local;
            usu = user;
            cl = cli;
            m_Bytes = bt;
            entryCargo.Text = loc.RespCargo;
            entryEmail.Text = loc.RespEmail;
            entryNome.Text = loc.RespNome;
            entryTelefone.Text = loc.RespTelefone;
            if (loc.Assinatura != null)
            {
                inputEntry_assinatura.Source = ImageSource.FromStream(() => new MemoryStream(loc.AssinaturaArray));
            }
            if (m_Bytes == null)
            {
                m_Bytes = loc.AssinaturaArray;
            }
            if (m_Bytes != null)
            {
                inputEntry_assinatura.Source = ImageSource.FromStream(() => new MemoryStream(m_Bytes));
            };

            button_assinatura.Clicked += delegate
        {
            loc.RespCargo = entryCargo.Text;
            loc.RespNome = entryNome.Text;
            loc.RespEmail = entryEmail.Text;
            loc.RespTelefone = entryTelefone.Text;
            //loc.IdAuditoria = loc.IdLocal;
            var navigationPage = new SignaturePadConfigView(null, usu, cl, loc, "local");

            App.NP.Popped += (sender, args) =>
            {
                (args.Page.BindingContext as BaseViewModel)?.OnDisappearing();
            };
            App.NP.Pushed += (sender, args) =>
            {
                (args.Page.BindingContext as BaseViewModel)?.OnAppearing();
            };
            App.NP.Navigation.PushAsync(navigationPage);
        };
        }

        async void okClicked(object sender, EventArgs e)
        {
            String timeStamp = MontaNomeFoto(DateTime.Now);
            if (!string.IsNullOrEmpty(entryNome.Text) && !string.IsNullOrEmpty(entryEmail.Text) && !string.IsNullOrEmpty(entryCargo.Text) && m_Bytes != null)
            {
                if (loc.Assinatura == null)
                {
                    loc.Assinatura = timeStamp + ".png";
                }
                imagem assinatura = new imagem()
                {
                    NomeImagem = loc.Assinatura,
                    ArrayBytes = loc.AssinaturaArray,
                    Tipo = "AssinaturaLocal"
                };

                Atualizar atualizar = new Atualizar();

                using (var dados = new DataAccess())
                {
                    vistoria_temp temp = dados.GetVistoriaTemp(loc.IdLocal);
                    if (temp != null)
                    {
                        if (string.IsNullOrEmpty(temp.HoraFim))
                        {
                            temp.HoraFim = GetTimestamp(DateTime.Now, "hora");
                            dados.InserirAtualizarVistoriaTemp(temp);
                        }
                    }
                    loc.RespCargo = entryCargo.Text;
                    loc.RespNome = entryNome.Text;
                    loc.RespEmail = entryEmail.Text;
                    loc.RespTelefone = entryTelefone.Text;
                    loc.Assinatura = loc.Assinatura;
                    dados.AtualizarLocais(loc);
                }
                //var internet = CrossConnectivity.Current.IsConnected;
                //if (internet)
                //{
                //    var result = await atualizar.EnviarImagem(assinatura);
                //    if (result != null && result != "HTTP error")
                //    {
                //        await DisplayAlert("Sucesso", "Dados atualizados com sucesso!", "Ok");
                //    }
                //    else
                //    {
                //        await DisplayAlert("Falha", "Houve um erro com o servidor tente novamente mais tarde", "Ok");
                //    }
                //}
                //else
                //{
                //    await DisplayAlert("Alerta", "Não existe conexão com a internet, conecte-se e tente novamente. Sua assinatura foi salva no celular.", "Ok");
                //}
                await DisplayAlert("Sucesso", "Dados atualizados com sucesso!", "Ok");
                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);

            }
            else if (string.IsNullOrEmpty(entryNome.Text))
            {
                await DisplayAlert("Alerta!", "Preencha o campo de nome", "Ok");
                entryNome.Focus();
            }
            else if (string.IsNullOrEmpty(entryEmail.Text))
            {
                await DisplayAlert("Alerta!", "Preencha o campo de email", "Ok");
                entryEmail.Focus();
            }
            else if (string.IsNullOrEmpty(entryCargo.Text))
            {
                await DisplayAlert("Alerta!", "Preencha o campo de cargo", "Ok");
                entryCargo.Focus();
            }
            else if (m_Bytes == null)
            {
                await DisplayAlert("Alerta!", "Preencha a assinatura", "Ok");
                loc.RespCargo = entryCargo.Text;
                loc.RespNome = entryNome.Text;
                loc.RespEmail = entryEmail.Text;
                loc.RespTelefone = entryTelefone.Text;
                //loc.IdAuditoria = loc.IdLocal;
                var navigationPage = new SignaturePadConfigView(null, usu, cl, loc, "local");
                await App.NP.Navigation.PushAsync(navigationPage);
            }
        }
        async void closeClicked(object sender, EventArgs e)
        {
            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
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

        public static String MontaNomeFoto(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}