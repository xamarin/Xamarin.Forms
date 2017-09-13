using Xamarin.Forms.Controls.Monkeys.ViewModels;

namespace Xamarin.Forms.Controls.Monkeys.Views
{
    public partial class MonkeysView : ContentPage
    {
        public MonkeysView()
        {
            InitializeComponent();

            BindingContext = new MonkeysViewModel();
        }
    }
}