using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler.Cells
{
    public class ItemMotivoCell : ViewCell
    {
        public ItemMotivoCell()
        {

            var itemLabel = new Label
            {
                FontSize = 16,
                TextColor = Color.FromHex("#384D79"),
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var stackLayout = new StackLayout
            {
                Children = { itemLabel },
                BackgroundColor = Color.FromHex("#E8EAEB"),
                Padding = new Thickness(5, 5, 5, 5)
            };

            View = new StackLayout
            {
                Children = { stackLayout },

            };
        }
    }
}
