using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8251, "Show snackbar", PlatformAffected.All)]
	public class Issue8251 : ContentPage
	{
		Label _labelResult;
		public Issue8251()
		{
			var button1 = new Button
			{
				Text = "Show snackbar with action button"
			};

			button1.Clicked += Button1_Clicked;

			var button2 = new Button
			{
				Text = "Show snackbar (no action button)"
			};

			button2.Clicked += Button2_Clicked;

			_labelResult = new Label();

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = { button1, button2, _labelResult }
			};
		}

		async void Button1_Clicked(object sender, EventArgs args)
		{
			var result = await DisplaySnackbar(GenerateLongText(5), 3000, "Run action", () =>
			{
				Debug.WriteLine("Snackbar action button clicked");
				return Task.CompletedTask;
			});
			_labelResult.Text = result ? "Snackbar is closed by user" : "Snackbar is closed by timeout";
		}

		async void Button2_Clicked(object sender, EventArgs args)
		{
			var result = await DisplaySnackbar(GenerateLongText(5));
			_labelResult.Text = result ? "Snackbar is closed by user" : "Snackbar is closed by timeout";
		}

		string GenerateLongText(int stringDuplicationTimes)
		{
			const string snackbarMessage = "It is a very long Snackbar mesage to test multiple strings. A B C D E F G H I I J K LO P Q R S T U V W X Y Z";
			var result = new StringBuilder();
			for (int i = 0; i < stringDuplicationTimes; i++)
			{
				result.AppendLine(snackbarMessage);
			}

			return result.ToString();
		}
	}
}
