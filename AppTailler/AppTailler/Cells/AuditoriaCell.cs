using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler.Cells
{
    public class AuditoriaCell : ViewCell
    {
        public AuditoriaCell()
        {
            var nomeLabel = new Label
            {
                FontSize = 36,
                TextColor = Color.White,
                Text = "Area",
                Rotation = -540,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
            };
            View = new StackLayout
            {
                Children = { nomeLabel },
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#5b5b5e"),
                WidthRequest = 250,
                Rotation = 270

            };
        }
    }
}
