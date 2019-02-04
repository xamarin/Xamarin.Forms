#if APP
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3454, "Picker accessibility text is wrong", PlatformAffected.macOS)]
	public class Issue3454 : TestContentPage
	{
		protected override void Init()
		{
			Visual = VisualMarker.Material;
			var pickers = new List<Picker> {
				new Picker(),
				new Picker(),
				new Picker { Title = "Title1" },
				new Picker { Title = "Title2" },
				new Picker { Visual = VisualMarker.Material },
				new Picker { Visual = VisualMarker.Material },
				new Picker { Title = "Title1", Visual = VisualMarker.Material },
				new Picker { Title = "Title2", Visual = VisualMarker.Material },
			};

			pickers[0].SetAutomationPropertiesName("First accessibility");
			pickers[0].SetAutomationPropertiesHelpText("This is the accessibility text");
			pickers[3].SetAutomationPropertiesName("Last accessibility");
			pickers[3].SetAutomationPropertiesHelpText("This is the accessibility text");

			// material
			pickers[4].SetAutomationPropertiesName("First accessibility");
			pickers[4].SetAutomationPropertiesHelpText("This is the accessibility text");
			pickers[7].SetAutomationPropertiesName("Last accessibility");
			pickers[7].SetAutomationPropertiesHelpText("This is the accessibility text");

			pickers.ForEach(p => p.ItemsSource = Enumerable.Range(1, 10).Select(i => $"item {i}").ToList());

			Content = new StackLayout
			{
				Children = {
					new Label { Text = "AutomationProperties" },
					pickers[0],
					new Label { Text = "Default" },
					pickers[1],
					new Label { Text = "Default + Title" },
					pickers[2],
					new Label { Text = "AutomationProperties + Title" },
					pickers[3],
					new Label { Text = "-=[ Material ]=-" },
					pickers[4],
					new Label { Text = "Default" },
					pickers[5],
					new Label { Text = "Default + Title" },
					pickers[6],
					new Label { Text = "AutomationProperties + Title" },
					pickers[7],
					new Button
					{
						Text = "Clear pickers",
						Command = new Command(() => pickers.ForEach(p => p.SelectedItem = null))
					}
				}
			};
		}
	}
}
#endif