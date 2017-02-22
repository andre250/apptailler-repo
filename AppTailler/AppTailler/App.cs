using AppTailler.Views;
using System;
using Xamarin.Forms;
using AppTailler.ViewModels;
using System.Threading.Tasks;
using System.Reflection;
using AppTailler.Models;
using Plugin.Connectivity;
using AppTailler.Pages;
using System.Collections.Generic;

namespace AppTailler
{
    public class App : Application
    {
        public static motivos motivoSelected { get; set; }
        public static settings GLOBAL_SETS { get; set; }
        public static NavigationPage NP { get; set; }
        public static MasterDetailPage MP { get; set; }
        public static bool Carregando { get; set; }
        private usuarios us;
        public App()
        {
            App.Carregando = false;
            NP = new NavigationPage();

            using (var dados = new DataAccess())
            {
                GLOBAL_SETS = dados.GetSettings(1);
            }
            if (GLOBAL_SETS != null)
            {
                if (GLOBAL_SETS.isPersist)
                {
                    int usuid = GLOBAL_SETS.lastLogin_userId;
                    using (var usu = new DataAccess())
                    {

                        us = usu.GetUsuarios(GLOBAL_SETS.lastLogin_userId);
                    }
                    NP.PushAsync(new Cliente(us, null));
                }
                else
                {
                    NP.PushAsync(new Login());
                }
            }
            else
            {
                NP.PushAsync(new Login());
            }

            //Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(5), () =>{ return true; });
            MainPage = NP;
        }
        public static new App Current => (App)Application.Current;

        //public static Task<string> DisplayActions(string cancel, string destruction, string buttons)
        //{
        //    var currentPage = ((NavigationPage)Current.MainPage).CurrentPage;
        //    return currentPage.DisplayActionSheet("Alerta", cancel, destruction, buttons);
        //}

        //public static Task<bool> DisplayAlert(string message, string yes = "OK", string no = null)
        //{
        //    var currentPage = ((NavigationPage)Current.MainPage).CurrentPage;
        //    if (no == null)
        //    {
        //        return currentPage.DisplayAlert("Alerta", message, yes).ContinueWith(task => true);
        //    }
        //    else
        //    {
        //        return currentPage.DisplayAlert("Alerta", message, yes, no);
        //    }
        //}

        public static Task NavigateTo<T>()
            where T : BaseViewModel
        {

            var viewModelName = typeof(T).Name;
            var pageType = typeof(MainView);
            var pageNamespace = pageType.Namespace;
            var pageAssembly = pageType.GetTypeInfo().Assembly;
            var newPageName = viewModelName.Substring(0, viewModelName.Length - "Model".Length);
            var newPageType = pageAssembly.GetType($"{pageNamespace}.{newPageName}");

            var newPage = Activator.CreateInstance(newPageType) as Page;
            var currentPage = ((NavigationPage)Current.MainPage).CurrentPage;
            return currentPage.Navigation.PushAsync(newPage);
        }

        public async Task<bool> IsConnected()
        {
            return CrossConnectivity.Current.IsConnected &&
                   await CrossConnectivity.Current.IsRemoteReachable("google.com");
        }
    }
}
