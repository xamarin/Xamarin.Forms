using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4459, "[UWP] BoxView CornerRadius doesn't work", PlatformAffected.UWP)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Issue4459 : ContentPage
	{
		public Issue4459()
		{
			InitializeComponent();
		}

		void InputView_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			BoxView.CornerRadius = new CornerRadius(double.Parse(TopLeft.Text), double.Parse(TopRight.Text),
			double.Parse(BottomLeft.Text), double.Parse(BottomRight.Text));
		}
	}

	public class Issue4459BoxView : BoxView
	{

	}
}