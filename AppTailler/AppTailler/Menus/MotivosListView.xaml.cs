using AppTailler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace AppTailler.Menus
{
    public partial class MotivosListView : ContentPage
    {
        public MotivosListView(List<motivos> motivos)
        {
            InitializeComponent();
            selecionar.ItemsSource = motivos;
            selecionar.HasUnevenRows = true;
            selecionar.ItemTemplate = new DataTemplate(() =>
            {
                var Button = new Button
                {
                    FontSize = 16,
                    BackgroundColor = Color.White,
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                Button.SetBinding(Button.TextProperty, "txtTexto");
                Button.SetBinding(Button.CommandParameterProperty, ".");
                Button.Command = new Command<motivos>((motivos itemSelecionado) =>
                {
                    App.motivoSelected = itemSelecionado;
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                });

                var stackLayout = new StackLayout
                {
                    Children = { Button },
                    BackgroundColor = Color.FromHex("#FFFFFF"),
                    Padding = new Thickness(5, 5, 5, 5),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Children = { stackLayout },
                    },
                };
            });
            //foreach (motivos nome in motivos)
            //{
            //    if (nome.txtTexto.Length >= 60)
            //    {
            //        nome.txtTexto = nome.txtTexto.Substring(0, 50) + "...";
            //    }
            //    selecionar.Items.Add(nome.txtTexto);
            //};

        }
    }
}
