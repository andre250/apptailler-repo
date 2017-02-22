using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler.Cells
{
    public class ItemAuditoriaCell : ViewCell
    {
        public ItemAuditoriaCell()
        {

            var itemLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 16,
                TextColor = Color.FromHex("#5B5B5E"),
                Text = "Aqui vem os itens das áreas.",
                WidthRequest = 300
            };

            var botConfirm = new Button
            {
                WidthRequest = 40,
                HeightRequest = 40,
                BackgroundColor = Color.Transparent,

            };

            var imgConfirm = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                Source = "confirm_null.png"
            };
            botConfirm.Clicked += delegate
            {
                imgConfirm.Source = "confirm_on.png";
            };

            var botUnconfirm = new Button
            {
                WidthRequest = 40,
                HeightRequest = 40,
                BackgroundColor = Color.Transparent
            };

            var imgUnconfirm = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                Source = "unconfirm_null.png"
            };
            botUnconfirm.Clicked += delegate
            {
                imgUnconfirm.Source = "unconfirm_on.png";
            };


            var botNa = new Button
            {
                WidthRequest = 40,
                HeightRequest = 40,
                BackgroundColor = Color.Transparent
            };

            var imgNa = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                Source = "na_null.png"
            };
            botNa.Clicked += delegate
            {
                imgNa.Source = "na_on.png";
            };


            AbsoluteLayout.SetLayoutFlags(itemLabel,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutFlags(botConfirm,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutFlags(imgConfirm,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutFlags(botUnconfirm,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutFlags(imgUnconfirm,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutFlags(botNa,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutFlags(imgNa,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(itemLabel,
                new Rectangle(0.2,
                    0.3, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            AbsoluteLayout.SetLayoutBounds(botConfirm,
                new Rectangle(0.7,
                    0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            AbsoluteLayout.SetLayoutBounds(imgConfirm,
                new Rectangle(0.7,
                    0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            AbsoluteLayout.SetLayoutBounds(botUnconfirm,
                new Rectangle(0.85,
                    0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            AbsoluteLayout.SetLayoutBounds(imgUnconfirm,
                new Rectangle(0.85,
                    0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            AbsoluteLayout.SetLayoutBounds(botNa,
                new Rectangle(1,
                    0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            AbsoluteLayout.SetLayoutBounds(imgNa,
                new Rectangle(1,
                    0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            var absoluteLayout = new AbsoluteLayout
            {
                Children = { itemLabel, botConfirm, imgConfirm, botUnconfirm, imgUnconfirm, botNa, imgNa },
            };

            View = new StackLayout
            {
                Children = { absoluteLayout },
                Orientation = StackOrientation.Vertical,

            };
        }
    }
}
