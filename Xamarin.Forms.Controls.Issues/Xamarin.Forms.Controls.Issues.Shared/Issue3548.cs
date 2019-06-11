using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Controls.Effects;
using System.Threading.Tasks;


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3548, "Cannot attach effect to Frame", PlatformAffected.Android, NavigationBehavior.PushAsync)]
	public class Issue3548 : ContentPage
	{
		private Frame _statusFrame;
		private AttachedStateEffect _effect;

		public Issue3548()
		{
			var statusLabel = new Label
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontSize = 40,
				Text = "EFFECT IS NOT ATTACHED"
			};

			_effect = new AttachedStateEffect();

			_statusFrame = new Frame
			{
				BackgroundColor = Color.Red,
				Padding = 15,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Content = statusLabel
			};

			_effect.StateChanged += (sender, e) =>
			{
				statusLabel.Text = _effect.State == AttachedStateEffect.AttachedState.Attached
					? "EFFECT IS ATTACHED!"
					: "EFFECT IS DEATTACHED";

				_statusFrame.BackgroundColor = Color.LightGreen;
			};

			Content = new StackLayout
			{
				Padding = 50,
				Children = {
					_statusFrame
				}
			};
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await Task.Delay(500);
			_statusFrame.Effects.Add(_effect);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_statusFrame.Effects.Remove(_effect);
		}
	}
}

