using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace AppTailler
{
    public partial class AuditoriaFinalizada : ContentPage
    {
        public AuditoriaFinalizada()
        {
            InitializeComponent();
            BackgroundColor = new Color(0, 0, 0, 0.5);
        }

        async void OnActionSheetSimpleClicked(object sender, EventArgs e)
        {
            await App.NP.Navigation.PopModalAsync();
        }
    }
}