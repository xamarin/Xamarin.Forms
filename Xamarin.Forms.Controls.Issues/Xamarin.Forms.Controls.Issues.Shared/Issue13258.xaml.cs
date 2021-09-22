using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13258, "[Bug] After PopModalAsync will not call OnAppearing (iOS)", PlatformAffected.iOS)]
	public partial class Issue13258 : TestContentPage
	{
		public Issue13258()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}

		void PushModal_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushModalAsync(new Issue13258_ModalPage());
		}

		void PushModalWithBackground_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushModalAsync(new Issue13258_ModalPage()
			{
				BackgroundColor = Color.Red,
			});
		}

		protected override void OnAppearing()
		{
			System.Diagnostics.Debug.WriteLine("Issue13258 OnAppearing");
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			System.Diagnostics.Debug.WriteLine("Issue13258 OnDisappearing");
			base.OnDisappearing();
		}

		public class Issue13258_ModalPage : ContentPage
		{
			public Issue13258_ModalPage()
			{
				Title = "Issue13258 ModalPage";

				var stackLayout = new StackLayout()
				{
					VerticalOptions = LayoutOptions.Center,
				};

				var button = new Button()
				{
					Text = "PopModal",
					VerticalOptions = LayoutOptions.Center,
				};
				button.Clicked += (sender, e) =>
				{
					Navigation.PopModalAsync();
				};

				stackLayout.Children.Add(button);

				Content = stackLayout;
			}

			protected override void OnAppearing()
			{
				System.Diagnostics.Debug.WriteLine("Issue13258_ModalPage OnAppearing");
				base.OnAppearing();
			}

			protected override void OnDisappearing()
			{
				System.Diagnostics.Debug.WriteLine("Issue13258_ModalPage OnDisappearing");
				base.OnDisappearing();
			}
		}
	}
}