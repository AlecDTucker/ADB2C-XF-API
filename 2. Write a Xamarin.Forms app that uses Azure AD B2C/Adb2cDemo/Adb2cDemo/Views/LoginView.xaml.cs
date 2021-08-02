using Adb2cDemo.ViewModels;
using Xamarin.Forms;

namespace Adb2cDemo.Views
{
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }
    }
}
