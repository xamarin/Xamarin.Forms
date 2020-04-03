using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4090, "Talkback reports incorrect number of list items", PlatformAffected.Android)]
	public class Issue4090 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor

			List<string> menuItems = new List<string>
			{
				"Browse","About"
			};

			StackLayout stack = new StackLayout();

			ListView ListViewMenu = new ListView();
			ListViewMenu.ItemsSource = menuItems;

			ListViewMenu.SelectedItem = menuItems[0];

			Label label = new Label();
			label.Text = "When clicking 'Browse' or 'About', Talkback should report that the element is in list of 2 items";
			stack.Children.Add(label);
			stack.Children.Add(ListViewMenu);
			Content = stack;


			BindingContext = new ViewModelIssue4090();
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue4090
	{
		public ViewModelIssue4090()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class ModelIssue4090
	{
		public ModelIssue4090()
		{

		}
	}
}