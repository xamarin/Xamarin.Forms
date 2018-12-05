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
			var picker = new Picker();
			var picker2 = new Picker();
#if !UITEST
			picker.SetAutomationPropertiesName("pi pi picker");
			picker.SetAutomationPropertiesHelpText("double tap to edit");
#endif
			picker.ItemsSource = picker2.ItemsSource = Enumerable.Range(1, 10).Select(i => $"item {i}").ToList();

			Content = new StackLayout
			{
				Children = {
					new Label { Text = "↓ Picker with setted AutomationProperties ↓" },
					picker,
					new Label { Text = "↓ Default picker ↓" },
					picker2
				}
			};
		}
	}
}
