using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3106, "LineBreakMode for Button", PlatformAffected.Default)]
	public class Github3106 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var stackLayout = new StackLayout()
			{
				Spacing = 10
			};

			var buttonNoWrap = new Button()
			{
				LineBreakMode = LineBreakMode.NoWrap,
				Text = "Button Button Button Button Button Button Button Button Button Button Button Button Button Button Button ",
				BackgroundColor = Color.SteelBlue,
				TextColor = Color.White,
				Padding = new Thickness(15)
			};

			stackLayout.Children.Add(buttonNoWrap);

			var buttonWordWrap = new Button()
			{
				LineBreakMode = LineBreakMode.WordWrap,
				Text = "Button Button Button Button Button Button Button Button Button Button Button Button Button Button Button ",
				BackgroundColor = Color.SteelBlue,
				TextColor = Color.White,
				Padding = new Thickness(50)
			};

			stackLayout.Children.Add(buttonWordWrap);

			var buttonCharacterWrap = new Button()
			{
				LineBreakMode = LineBreakMode.CharacterWrap,
				Text = "Button Button Button Button Button Button Button Button Button Button Button Button Button Button Button ",
				BackgroundColor = Color.SteelBlue,
				TextColor = Color.White,
				Padding = new Thickness(15)
			};

			stackLayout.Children.Add(buttonCharacterWrap);

			var buttonHeadTruncation = new Button()
			{
				LineBreakMode = LineBreakMode.HeadTruncation,
				Text = "Button Button Button Button Button Button Button Button Button Button Button Button Button Button Button ",
				BackgroundColor = Color.SteelBlue,
				TextColor = Color.White,
				Padding = new Thickness(15)
			};

			stackLayout.Children.Add(buttonHeadTruncation);

			var buttonTailTruncation = new Button()
			{
				LineBreakMode = LineBreakMode.TailTruncation,
				Text = "Button Button Button Button Button Button Button Button Button Button Button Button Button Button Button ",
				BackgroundColor = Color.SteelBlue,
				TextColor = Color.White,
				Padding = new Thickness(15)
			};

			stackLayout.Children.Add(buttonTailTruncation);

			var buttonMiddleTruncation = new Button()
			{
				LineBreakMode = LineBreakMode.MiddleTruncation,
				Text = "Button Button Button Button Button Button Button Button Button Button Button Button Button Button Button ",
				BackgroundColor = Color.SteelBlue,
				TextColor = Color.White,
				Padding = new Thickness(15)
			};

			stackLayout.Children.Add(buttonMiddleTruncation);

			Content = stackLayout;
		}
	}
}