using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7992, "Datepicker is not opened when we call Datepicker.Focus() in UWP", PlatformAffected.UWP)]

	public class Issue7992 : TestContentPage
	{
		protected override void Init()
		{
			var stackLayout = new StackLayout();
			Content = stackLayout;

			stackLayout.Children.Add(new Label { Text = "Label to keep picker from getting initial focus" });

			var datePicker = new DatePicker();
			stackLayout.Children.Add(datePicker);

			var button = new Button { Text = "Focus Picker" };
			button.Clicked += (s, e) =>
			{
				datePicker.Focus();
			};
			stackLayout.Children.Add(button);
		}

	}
}
