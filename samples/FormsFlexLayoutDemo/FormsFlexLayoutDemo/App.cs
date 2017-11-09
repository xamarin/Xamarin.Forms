using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FormsFlexLayoutDemo
{
    public class App : Application
    {
        public App()
        {
            MainPage = new MasterDetailPage
            {
                Master = new FlexOptionsPage(),
                Detail = new NavigationPage(new FlexDemoPage()),
                BindingContext = new FlexLayoutViewModel()
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
