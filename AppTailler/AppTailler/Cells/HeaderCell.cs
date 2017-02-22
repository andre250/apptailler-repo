using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler.Cells
{
    public class HeaderCell : ViewCell
    {
        public HeaderCell()
        {
            var title = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, this),
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center
            };

            title.SetBinding(Label.TextProperty, "ItemDescricao");

            View = new StackLayout
            {
                Padding = new Thickness(10, 10, 10, 10),
                BackgroundColor = Color.FromHex("#384D79"),
                Children = { title }
            };
        }
    }
}
