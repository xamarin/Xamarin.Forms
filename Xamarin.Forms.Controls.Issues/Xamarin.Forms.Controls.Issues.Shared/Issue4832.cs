using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4832, "Editor crashes on return type when next element is not editable", PlatformAffected.Android)]
	public class Issue4832 : ContentPage
	{
		public Issue4832()
		{
			Title = "Test page";
			Padding = 10;

			var layout = new StackLayout()
			{
				Children = {
					new Editor(),
					new Button() { Text = "Btn" }
				}
			};

			Content = layout;
		}
	}
}
