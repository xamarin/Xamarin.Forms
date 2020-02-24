using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.CustomAttributes;
using System;
using System.Windows.Input;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CarouselView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8781, "SwipeViewItem rendering issue on iOS", PlatformAffected.iOS)]
	public partial class Issue8781 : ContentPage
	{
		public Issue8781()
		{
#if APP
			InitializeComponent();
			BindingContext = this;
#endif
		}

		public ICommand CheckAnswerCommand => new Command<string>(OnCheckAnswer);

		async void OnCheckAnswer(string parameter)
		{
			if (string.IsNullOrEmpty(parameter))
				return;

			if (parameter.Equals("4", StringComparison.InvariantCultureIgnoreCase))
			{
				resultEntry.Text = string.Empty;
				swipeView.Close();
				await DisplayAlert("Correct!", "The answer is 2.", "OK");
			}
			else
			{
				resultEntry.Text = string.Empty;
				await DisplayAlert("Incorrect!", "Try again.", "OK");
			}
		}
	}
}