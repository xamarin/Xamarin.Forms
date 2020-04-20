﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3667, "[Enhancement] Add text-transforms to Label", PlatformAffected.All)]
	public class Issue3667 : TestContentPage
	{
		string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

		protected override void Init()
		{
			var text2 = "Malesuada fames ac turpis egestas maecenas pharetra convallis. Dictum varius duis at consectetur lorem donec massa. Augue interdum velit euismod in pellentesque.";
			var transform = TextTransform.None;

			var list = new ITextElement[] {
				new Label { Text = text },
				new Entry { Text = "Entry text" },
				new Editor { Text = "Editor text" },
				new SearchBar { Text = "SearchBar text" },
				new Button { Text = "Button text" },
			};

			var statusLabel = new Label
			{
				Text = "Current TextTransform is None",
				BackgroundColor = Color.Aqua,
				TextTransform = TextTransform.None
			};
			var but = new Button
			{
				Text = "Change TextTransform",
				Command = new Command(() =>
				{
					if (++transform > TextTransform.Uppercase)
						transform = TextTransform.None;
					foreach (var item in list)
						item.TextTransform = transform;
					statusLabel.Text = $"Current TextTransform is {transform}";
				})
			};

			var layout = new StackLayout
			{
				Children = {
					statusLabel,
					but
				}
			};

			foreach (var item in list)
				layout.Children.Add(item as View);

			layout.Children.Add(new Label
			{
				Text = "[Lowercase] " + text2,
				TextTransform = TextTransform.Lowercase
			});
			layout.Children.Add(new Label
			{
				Text = "[Uppercase] " + text2,
				TextTransform = TextTransform.Uppercase
			});

			Content = layout;
		}

#if UITEST
		[Test]
		public void Issue3667Tests ()
		{
			var labelValue = RunningApp.Query(query => query.Text(text));
			if (labelValue.Length == 0)
				Assert.Fail();
			RunningApp.Tap("Change TextTransform");
			labelValue = RunningApp.Query(query => query.Text(text.ToLowerInvariant()));
			if (labelValue.Length == 0)
				Assert.Fail();
			RunningApp.Tap("Change TextTransform");
			labelValue = RunningApp.Query(query => query.Text(text.ToUpperInvariant()));
			if (labelValue.Length == 0)
				Assert.Fail();
			RunningApp.Tap("Change TextTransform");
			labelValue = RunningApp.Query(query => query.Text(text));
			if (labelValue.Length == 0)
				Assert.Fail();
		}
#endif
	}
}


