using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace AppTailler.Menus
{
    public partial class Loading : ContentPage
    {
        public Loading(string labelText)
        {
            InitializeComponent();
            Label.Text = labelText;
        }
    }
}
