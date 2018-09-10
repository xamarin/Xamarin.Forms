using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 1454, "Disabled button passes user tap to control behind it")]
	public class Issue1454 : TestContentPage
	{
		Label statusLabel;

		void SetStatus(string status)
		{
			Debug.WriteLine(status);
			statusLabel.Text = status;
		}

		protected override void Init ()
		{
			statusLabel = new Label { Text = "Status" };
			var Items = new ObservableCollection<string>(Enumerable.Range(0, 10).Select(x => $"Item {x}"));
			var template = new DataTemplate(() => new ViewCell()
			{
				Height = 30,
				View = new StackLayout()
				{
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand,
					Children =
					{
						new Button
						{
							Text = "Click me",
							Command = new Command(() => SetStatus("Button Hit")),
							IsEnabled = false,
						}
					}
				}
			});

			var lstView = new ListView
			{
				ItemsSource = Items,
				ItemTemplate = template,
				IsPullToRefreshEnabled = true,
			};

			lstView.ItemTapped += (_, __) => SetStatus("List View Tapped");

			Content = new StackLayout
			{
				Children = {
					statusLabel,
					lstView
				}
			};
		}
	}
}
