using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using AppTailler.Pages;
using AppTailler.Models;
using Plugin.Connectivity;
using Xamarin.Forms.Xaml;
using Security;
using Foundation;
using Android.OS;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AppTailler
{
    public partial class Login : ContentPage, INotifyPropertyChanged
    {
        private usuarios us;
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
        #region Main
        public Login()
        {
            IsLoading = false;
            BindingContext = this;
            InitializeComponent();
            //Busca os settings armazenados no banco de dados e armazena em uma variavel de sessão
            using (var dados = new DataAccess())
            {
                App.GLOBAL_SETS = dados.GetSettings(1);
            }
            //Retira a barra de navegação da página
            NavigationPage.SetHasNavigationBar(this, false);

            subBot.Clicked += SubBot_Clicked;

            var btTut = new TapGestureRecognizer();

            btTut.Tapped += delegate
            {
                TurnPersistent();
            };

            persistConnection.GestureRecognizers.Add(btTut);

            ValidarLogin();
        }
        #endregion

        #region Botao de login
        public async void SubBot_Clicked(object sender, EventArgs e)
        {
            IsLoading = true;
            using (var usu = new DataAccess())
            {

                if (string.IsNullOrEmpty(userInput.Text))
                {
                    await DisplayAlert("Alerta", "Campo de usuário em branco.", "Ok");
                    userInput.Focus();
                    return;
                }
                else if (string.IsNullOrEmpty(passInput.Text))
                {
                    await DisplayAlert("Alerta", "Campo de password em branco.", "Ok");
                    passInput.Focus();
                    return;
                }
                else
                {
                    string user, password;
                    user = userInput.Text;
                    password = passInput.Text;
                    //Antes do login verifica se existe internet
                    var internet = CrossConnectivity.Current.IsConnected;
                    if (!internet)
                    {
                        //Se nao existe internet ele puxa as informações da base
                        us = usu.GetUsuarios(App.GLOBAL_SETS.lastLogin_userId);
                        //Se nunca houve login ele dispara um alerta
                        if (us == null)
                        {
                            await
                                DisplayAlert("Alerta",
                                    "Você não possui conexão com a internet. É necessário fazer o Login pelo menos uma vez para acessar o Modo Offline.",
                                    "Ok");
                        }
                        //Se ja existia um login ele ativa o modo offline e carrega a proxima pagina
                        else
                        {
                            if (user == us.usuUsuario)
                            {
                                //Valida senha
                                if (password == us.usuSenha)
                                {
                                    await
                                        DisplayAlert("Alerta", "Você não possui conexão com a internet. Modo Offline: ON",
                                            "Ok");
                                    App.GLOBAL_SETS.modoOffline = true;
                                    await App.NP.Navigation.PushAsync(new RootPage(us, null, null, "cliente", 1));
                                }
                                else
                                {
                                    await DisplayAlert("Alerta", "Senha incorreta", "Ok");
                                }
                            }
                            else
                            {
                                us = usu.GetUsuariosByLogin(userInput.Text);
                                if (us == null)
                                {
                                    await
                                DisplayAlert("Alerta",
                                    "Você não possui conexão com a internet. É necessário fazer o Login pelo menos uma vez para acessar o Modo Offline.",
                                    "Ok");
                                }
                                else
                                {
                                    if (password == us.usuSenha)
                                    {
                                        await
                                        DisplayAlert("Alerta", "Você não possui conexão com a internet. Modo Offline: ON",
                                            "Ok");
                                        App.GLOBAL_SETS.lastLogin_user = us.usuUsuario;
                                        App.GLOBAL_SETS.lastLogin_userId = us.IdPessoa;
                                        App.GLOBAL_SETS.modoOffline = true;
                                        usu.AtualizarSettings(App.GLOBAL_SETS);
                                        await App.NP.Navigation.PushAsync(new RootPage(us, null, null, "cliente", 1));
                                    }
                                    else
                                    {
                                        await DisplayAlert("Alerta", "Senha incorreta", "Ok");
                                    }
                                }
                            }
                        }
                    }
                    else
                    //Se existe internet ele faz a atualização online
                    {
                        App.GLOBAL_SETS.modoOffline = false;
                        Atualizar atualizar = new Atualizar();
                        var result = await atualizar.AtualizarUsuarios(user, password);
                        //Validação de senha e erros de servidor
                        if (result != "Sucesso")
                        {
                            await DisplayAlert("Alerta", result, "Ok");
                        }
                        //Se nao existe nenhum erro ele faz o login
                        else
                        {
                            us = usu.GetUsuarios(App.GLOBAL_SETS.lastLogin_userId);
                            await App.NP.Navigation.PushAsync(new RootPage(us, null, null, "cliente", 1));
                        }
                    }
                }
            }
            IsLoading = false;
        }
        #endregion

        #region TurnPersistent
        public async void TurnPersistent()
        {
            if (App.GLOBAL_SETS.isPersist == false)
            {
                App.GLOBAL_SETS.isPersist = true;
                persistConnection.Source = "checkselecionado.png";
            }
            else
            {
                App.GLOBAL_SETS.isPersist = false;
                persistConnection.Source = "checknormal.png";
            }
        }
        #endregion

        #region Verifica Internet

        public async Task<bool> IsConnected()
        {
            return CrossConnectivity.Current.IsConnected &&
                   await CrossConnectivity.Current.IsRemoteReachable("google.com");
        }

        #endregion

        #region ValidarLogin
        public async void ValidarLogin()
        {
            //Verifica se a varivel global esta nulla e insere os settings padrões caso seja o primeiro login
            if (App.GLOBAL_SETS == null)
            {
                App.GLOBAL_SETS = new settings() { ID = 1, isPersist = false, lastLogin_user = null, lastLogin_userId = 0, modoOffline = false };
                using (var dados = new DataAccess())
                {
                    dados.InserirSettings(App.GLOBAL_SETS);
                }
            }
            //Verifica o ultimo usuario que fez login, se não for nulo ele preence o campo de login
            if (string.IsNullOrEmpty(App.GLOBAL_SETS.lastLogin_user))
            {
                userInput.Text = App.GLOBAL_SETS.lastLogin_user;
            }
            //Verifica se houve alguma tentativa de login anterior e preenche o input de login
            if (App.GLOBAL_SETS.lastLogin_user != null)
            {
                userInput.Text = App.GLOBAL_SETS.lastLogin_user;
            }
            //Verifica o conexão de internet e antigo persistencia de login se ambos forem verdadeiros conecta e puxa informações do ultimo usuario conectado
            var internet = CrossConnectivity.Current.IsConnected;
            if (App.GLOBAL_SETS.isPersist && internet)
            {
                int usuid = App.GLOBAL_SETS.lastLogin_userId;
                using (var usu = new DataAccess())
                {
                    us = usu.GetUsuarios(App.GLOBAL_SETS.lastLogin_userId);
                }
                await App.NP.Navigation.PushAsync(new RootPage(us, null, null, "cliente", 1));
            }
        }

        #endregion

        public static String GetTimestamp(DateTime value)
        {
            
            return value.ToString("HH:mm:ss:ffff");
        }

    }
}
