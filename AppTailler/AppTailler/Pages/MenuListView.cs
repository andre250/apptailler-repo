using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler
{
    public class MenuListView : ListView
    {
        public MenuListView(int tela_atual)
        {
            List<MenuItem> data = new MenuListData(tela_atual);

            ItemsSource = data;
            VerticalOptions = LayoutOptions.Fill;
            BackgroundColor = Color.Transparent;
            SeparatorVisibility = SeparatorVisibility.None;
            Margin = new Thickness(20,0,20,20);

            var cell = new DataTemplate(typeof(MenuCell));
            
            cell.SetBinding(MenuCell.TextProperty, "Title");
            cell.SetBinding(MenuCell.ImageSourceProperty, "IconSource");
            cell.SetBinding(MenuCell.IsEnabledProperty, "IsEnabled");

            ItemTemplate = cell;
        }
    }
}
