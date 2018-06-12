using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "[macOS] Entry unfocus behavior", PlatformAffected.macOS)]
	public class EntryUnfocusedImmediately : TestContentPage
	{
		Label _focusedCountLabel = new Label
		{
			Text = "Focused count: 0"
		};
		int _focusedCount;
		int FocusedCount
		{
			get { return _focusedCount; }
			set
			{
				_focusedCount = value;
				_focusedCountLabel.Text = $"Focused count: {value}";
			}
		}

		Label _unfocusedCountLabel = new Label
		{
			Text = "Unfocused count: 0"
		};
		int _unfocusedCount;
		int UnfocusedCount
		{
			get { return _unfocusedCount; }
			set
			{
				_unfocusedCount = value;
				_unfocusedCountLabel.Text = $"Unfocused count: {value}";
			}
		}

		protected override void Init()
		{
			var entry = new Entry();
			entry.Focused += (sender, e) =>
			{
				FocusedCount++;
			};
			entry.Unfocused += (sender, e) =>
			{
				UnfocusedCount++;
			};

			var dumbyEntry = new Entry()
			{
				Placeholder = "I'm just here as another focus target"
			};

			var divider = new BoxView
			{
				HeightRequest = 1,
				BackgroundColor = Color.Black
			};

			StackLayout stackLayout = new StackLayout();
			stackLayout.Children.Add(dumbyEntry);
			stackLayout.Children.Add(divider);
			stackLayout.Children.Add(entry);
			stackLayout.Children.Add(_focusedCountLabel);
			stackLayout.Children.Add(_unfocusedCountLabel);

			Content = stackLayout;
		}
	}
}
