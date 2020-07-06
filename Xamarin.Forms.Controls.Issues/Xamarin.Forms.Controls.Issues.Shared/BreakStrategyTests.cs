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
			var grid = new Grid
			{
				RowDefinitions = new RowDefinitionCollection()
				{
					new RowDefinition(), new RowDefinition()
				}
			};

			var label = new Label
			{
				Text = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
				Margin = 4
			};

			var buttonStackLayout = new StackLayout();

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

			buttonStackLayout.Children.Add(button1);
			buttonStackLayout.Children.Add(button2);
			buttonStackLayout.Children.Add(button3);

			grid.AddChild(label, 0, 0);
			grid.AddChild(buttonStackLayout, 0, 1);

			Content = grid;
		}
	}
}
