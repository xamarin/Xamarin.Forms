using Xamarin.Forms.Controls.PullToRefresh.ViewModels;

namespace Xamarin.Forms.Controls
{
    public partial class PullToRefreshView : ContentPage
    {
        public PullToRefreshView()
        {
            InitializeComponent();

            BindingContext = new PullToRefreshViewModel();
        }
    }
}