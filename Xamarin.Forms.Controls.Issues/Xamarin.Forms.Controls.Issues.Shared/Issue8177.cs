using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif
namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8177, "[Bug] Picker does not update when it's underlying list changes content",
		PlatformAffected.UWP)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Picker)]
#endif
	public class Issue8177 : TestContentPage
	{
		protected override void Init()
		{
			var layout = new StackLayout();

			var button = new Button { Text = "Change Picker Contents " };
			
			var originalList = new List<string> { "one", "two", "three" };
			var picker = new Picker { ItemsSource = originalList };

			var newList = new List<string> { "uno", "dos", "tres", "quatro" };
			button.Clicked += (sender, args) => { picker.ItemsSource = newList; };

			layout.Children.Add(button);
			layout.Children.Add(picker);

			Content = layout;
		}
	}
}
