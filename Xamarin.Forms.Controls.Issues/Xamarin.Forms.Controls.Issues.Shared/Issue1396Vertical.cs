using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1396, 
		"Label TextAlignment is not kept when resuming application", 
		PlatformAffected.Android)]
	public class Issue1396Vertical : TestContentPage
	{
		Label _label;

		protected override void Init()
		{
			var instructions = new Label
			{
				Text = "Tap the 'Change Text' button. Tap the Overview button. Resume the application. If the label" 
						+ " text is no longer centered, the test has failed."
			};

			var button = new Button { Text = "Change Text" };
			button.Clicked += (sender, args) =>
			{
				_label.Text = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
			};

			_label = new Label
			{ 
				HeightRequest = 400,
				BackgroundColor = Color.Gold,
				Text = "I should be centered in the gold area",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var layout = new StackLayout 
			{
				Children =
				{
					instructions, 
					button,
					_label
				}
			};

			var content = new ContentPage 
			{
				Content = layout 
			};

			Content = new Label { Text = "Shouldn't see this" };

			Appearing += (sender, args) => Application.Current.MainPage = content;
		}
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1415, 
		"HorizontalTextAlignment=\"Center\" loses alignment when scrolled", 
		PlatformAffected.Android)]
	public class Issue1415 : TestContentPage
	{
		[Preserve(AllMembers = true)]
		public class _1415ViewModel
		{
			public ICommand AddMoreCommand { get; set; }
			public ObservableCollection<_1415Item> ListViewItems { get; set; }

			public _1415ViewModel()
			{
				ListViewItems = new ObservableCollection<_1415Item>();
				for (int i = 0; i < 500; i++)
				{
					ListViewItems.Add(new _1415Item() { PropA = $"A {i}" });
				}

				AddMoreCommand = new Command(() => { ListViewItems.Insert(0, new _1415Item() { PropA = "New!" }); });

			}
		}

		[Preserve(AllMembers = true)]
		public class _1415Item
		{
			public string PropA { get; set; }
		}

		protected override void Init()
		{
			BindingContext = new _1415ViewModel();

			var layout = new StackLayout();

			var button = new Button { Text = "Add More!" };
			button.SetBinding(Button.CommandProperty, "AddMoreCommand");
			layout.Children.Add(button);

			var lv = new ListView();
			lv.SetBinding(ListView.ItemsSourceProperty, "ListViewItems");

			lv.ItemTemplate = new DataTemplate(() =>
			{
				var frame = new Frame();

				var sl = new StackLayout();

				for (int n = 0; n < 4; n++)
				{
					var label = new Label { HorizontalTextAlignment = TextAlignment.Center };
					label.SetBinding(Label.TextProperty, "PropA");
					sl.Children.Add(label);
				}

				frame.Content = sl;
				var cell = new ViewCell { View = frame };
				return cell;
			});

			layout.Children.Add(lv);

			Content = layout;
		}
	}
}