using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler.Cells
{
    public class ItemAuditoriaHeader : ContentView
    {
        public ItemAuditoriaHeader()
        {

            var descLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Start,
                FontSize = 36,
                TextColor = Color.FromHex("#FFFFFF"),
                Text = "Descrição da Area"
            };


            var View = new StackLayout
            {
                Children = { descLabel },
                BackgroundColor = Color.Blue,
                Orientation = StackOrientation.Horizontal,

            };
        }
    }
}
