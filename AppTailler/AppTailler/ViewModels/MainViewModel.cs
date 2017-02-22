using System.Windows.Input;
using Xamarin.Forms;

namespace AppTailler.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand DataBindingCommand { get; } = new Command(() => App.NavigateTo<SignaturePadConfigViewModel>());
    }
}
