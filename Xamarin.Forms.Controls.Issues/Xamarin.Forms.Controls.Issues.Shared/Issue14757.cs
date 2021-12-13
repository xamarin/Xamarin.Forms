using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14757, "[Bug] Android ObjectDisposedException thrown Button inside CollectionView with Binding", PlatformAffected.Android)]
	public class Issue14757 : TestContentPage
	{
		public ObservableCollection<int> Values { get; set; } = new ObservableCollection<int>();

		public static readonly BindableProperty SomeTextProperty = BindableProperty.Create(nameof(SomeText), typeof(string), typeof(Issue14757), default(string));
		public string SomeText
		{
			get { return (string)GetValue(SomeTextProperty); }
			set { SetValue(SomeTextProperty, value); }
		}

		public Issue14757()
		{
			SomeText = "Default";

			for (int i = 0; i <= 1000; i++)
				Values.Add(i);

			BindingContext = this;
		}

		const string LabelAutomationId = "TheStatusLabel";
		const string GoToPageAutomationId = "GoToPageButton";
		const string ChangeTextAutomationId = "ChangeTextButton";

		protected override void Init()
		{
			var statusLabel = new Label()
			{
				AutomationId = LabelAutomationId
			};
			statusLabel.SetBinding(Label.TextProperty, new Binding("SomeText"));

			var button1 = new Button
			{
				Text = "To testpage",
				AutomationId = GoToPageAutomationId
			};
			button1.Clicked += Button1_Clicked;

			var button2 = new Button
			{
				Text = "Change Text",
				AutomationId = ChangeTextAutomationId
			};
			button2.Clicked += Button2_Clicked;

			var stackLayout = new StackLayout
			{
				Children = {
					statusLabel,
					button1,
					button2
				}
			};

			Content = stackLayout;
		}

		private void Button2_Clicked(object sender, EventArgs e)
		{
			SomeText = SomeText == "Default" ? "Some Value" : "Default";
		}

		private async void Button1_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new NewPage(this));
		}

		class NewPage : ContentPage
		{
			public Issue14757 MyMainPage { get; set; }

			public NewPage(Issue14757 mainpage)
			{
				BindingContext = MyMainPage = mainpage;

				var dataTemplate = new DataTemplate(() =>
				{
					var button = new Button();
					button.SetBinding(Button.TextProperty, new Binding("SomeText", source: MyMainPage));

					var stackLayout = new StackLayout
					{
						Children =
						{
							button
						}
					};

					return stackLayout;
				});

				var collectionView = new CollectionView();
				collectionView.SetBinding(CollectionView.ItemsSourceProperty, new Binding("Values"));
				collectionView.ItemTemplate = dataTemplate;

				var stackLayout = new StackLayout
				{
					Children =
					{
						collectionView
					}
				};

				Content = stackLayout;
			}
		}

#if UITEST
		[Test]
		public void Issue14757Test()
		{
			System.Threading.Tasks.Task.Delay(500);

			RunningApp.WaitForElement(LabelAutomationId);

			for (int i = 0; i <= 10; i++)
			{
				RunningApp.Tap(GoToPageAutomationId);
				System.Threading.Tasks.Task.Delay(500);
				RunningApp.Back();
			}

			RunningApp.Tap(ChangeTextAutomationId);
			RunningApp.WaitForElement("Some Value");
		}
#endif
	}
}
