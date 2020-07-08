using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Label)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "[Enhancement] Label: Break Strategy on Android", PlatformAffected.Android)]
	public class BreakStrategyTests
		: TestContentPage
	{
		protected override void Init()
		{
			Title = "Android Break Strategy";

			var grid = new Grid
			{
				RowDefinitions = new RowDefinitionCollection()
				{
					new RowDefinition(), new RowDefinition()
				},

				ColumnDefinitions = new ColumnDefinitionCollection()
				{
					new ColumnDefinition(), new ColumnDefinition()
				}
			};

			var label = new Label
			{
				Text = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
				Margin = 4
			};

			var buttonStackLayout1 = new StackLayout();

			var button1 = new Button
			{
				Text = "Simple"
			};

			button1.Clicked += (o, e) =>
			{
				label.On<PlatformConfiguration.Android>().SetBreakStrategy(BreakStrategyFlags.Simple);
			};

			var button2 = new Button
			{
				Text = "Balanced"
			};

			button2.Clicked += (o, e) =>
			{
				label.On<PlatformConfiguration.Android>().SetBreakStrategy(BreakStrategyFlags.Balanced);
			};

			var button3 = new Button
			{
				Text = "High Quality"
			};

			button3.Clicked += (o, e) =>
			{
				label.On<PlatformConfiguration.Android>().SetBreakStrategy(BreakStrategyFlags.HighQuality);
			};

			buttonStackLayout1.Children.Add(new Label()
			{
				Text = "Break Strategy:" ,
				HorizontalTextAlignment = TextAlignment.Center
			});

			buttonStackLayout1.Children.Add(button1);
			buttonStackLayout1.Children.Add(button2);
			buttonStackLayout1.Children.Add(button3);

			var buttonStackLayout2 = new StackLayout();

			var button4 = new Button
			{
				Text = "CharacterWrap"
			};

			button4.Clicked += (o, e) =>
			{
				label.LineBreakMode = LineBreakMode.CharacterWrap;
			};

			var button5 = new Button
			{
				Text = "HeadTruncation"
			};

			button5.Clicked += (o, e) =>
			{
				label.LineBreakMode = LineBreakMode.HeadTruncation;
			};

			var button6 = new Button
			{
				Text = "MiddleTruncation"
			};

			button6.Clicked += (o, e) =>
			{
				label.LineBreakMode = LineBreakMode.MiddleTruncation;
			};

			var button7 = new Button
			{
				Text = "NoWrap"
			};

			button7.Clicked += (o, e) =>
			{
				label.LineBreakMode = LineBreakMode.NoWrap;
			};

			var button8 = new Button
			{
				Text = "TailTruncation"
			};

			button8.Clicked += (o, e) =>
			{
				label.LineBreakMode = LineBreakMode.TailTruncation;
			};

			var button9 = new Button
			{
				Text = "WordWrap"
			};

			button9.Clicked += (o, e) =>
			{
				label.LineBreakMode = LineBreakMode.WordWrap;
			};

			buttonStackLayout2.Children.Add(new Label()
			{
				Text = "Line Break Mode:",
				HorizontalTextAlignment = TextAlignment.Center
			});

			buttonStackLayout2.Children.Add(button4);
			buttonStackLayout2.Children.Add(button5);
			buttonStackLayout2.Children.Add(button6);
			buttonStackLayout2.Children.Add(button7);
			buttonStackLayout2.Children.Add(button8);
			buttonStackLayout2.Children.Add(button9);

			grid.AddChild(label, 0, 0, 2);
			grid.AddChild(buttonStackLayout1, 0, 1);
			grid.AddChild(buttonStackLayout2, 1, 1);

			var scrollView = new ScrollView
			{
				Content = grid
			};

			Content = scrollView;
		}
	}
}
