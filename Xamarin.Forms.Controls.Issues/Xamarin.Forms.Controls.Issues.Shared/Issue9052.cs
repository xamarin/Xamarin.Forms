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
	[Issue(IssueTracker.Github, 9052, "[Bug][UWP] Switch takes up more horizontal space then before", PlatformAffected.UWP)]
	public class Issue9052 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 9052";

			StackLayout layout = new StackLayout();

			Label instructions = new Label
			{
				Text = "Check that the yellow background of the Switch below is only around the Switch and text - no extra space around."
			};

			Switch theSwitch = new Switch
			{
				BackgroundColor = Color.Yellow
			};

			layout.Children.Add(instructions);
			layout.Children.Add(theSwitch);

			Content = layout;
		}
	}
}
