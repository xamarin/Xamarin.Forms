using System;
using System.Linq.Expressions;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5890, "[macOS] Control with VerticalOption=Start is sticked to bottom", PlatformAffected.macOS)]
	public class Issue5890 : TestContentPage
	{
		
		protected override void Init()
		{
			var sp = new StackLayout {VerticalOptions = LayoutOptions.Start};
			sp.Children.Add(new Button
			{
				Text = "Change the height of the window. This button should be sticked to the top."
			});
			Content = sp;
		}

	}
}
