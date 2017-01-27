using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2659, "", PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue2659 : ContentPage
	{
		public Issue2659()
		{
			try
			{
				InitializeComponent();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
		}

		internal void OnClearLocalButtonClicked(object sender, EventArgs args)
		{
			EnumerateButtons((Button button) =>
			{
				button.ClearValue(Button.TextColorProperty);
				button.ClearValue(Button.FontAttributesProperty);
			});
		}

		internal void OnSetLocalButtonClicked(object sender, EventArgs args)
		{
			EnumerateButtons((Button button) =>
			{
				button.TextColor = Color.Red;
				button.FontAttributes = FontAttributes.Bold;
			});
		}

		internal void OnSetStyleButtonClicked(object sender, EventArgs args)
		{
			var style = (Style)Resources["buttonStyle"];
			SetButtonStyle(style);
		}

		internal void OnUnsetStyleButtonClicked(object sender, EventArgs args)
		{
			SetButtonStyle(null);
		}

		void EnumerateButtons(Action<Button> action)
		{
			foreach (View view in stackLayout.Children)
				action((Button)view);
		}

		void SetButtonStyle(Style style)
		{
			EnumerateButtons(button => { button.Style = style; });
		}
	}
#endif
}