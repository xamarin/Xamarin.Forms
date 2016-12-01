using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 48237, "ContextAction menu is automatically dismissed when rotating from portrait to landscape mode in a Xamarin.Forms application on iOS.", PlatformAffected.iOS)]
	public class Bugzilla48237 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var list = new List<int>();
			for (var i = 0; i < 15; i++)
				list.Add(i);

			var listView = new ListView
			{
				ItemsSource = list,
				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));

					return new ViewCell
					{
						View = new ContentView
						{
							Content = label,
						},
						ContextActions = { new MenuItem
						{
							Text = "Action"
						},
						new MenuItem
						{
							Text = "Delete",
							IsDestructive = true
						} }
					};
				})
			};

			Content = listView;
		}
	}
}