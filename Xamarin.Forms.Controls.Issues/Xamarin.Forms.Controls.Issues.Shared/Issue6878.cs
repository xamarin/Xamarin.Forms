using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Threading;
using System.ComponentModel;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6878,
		"ShellItem.Items.Clear() crashes when the ShellItem has bottom tabs", PlatformAffected.All)]

#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue6878 : TestShell
	{
		const string ExceptionMessage = "😢 oh no! An exception as throw...";
		const string ClearShellItems = "ClearShellItems";
		const string StatusLabel = "StatusLabel";

		StackLayout _stackContent;

		protected override void Init()
		{
			_stackContent = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						AutomationId = StatusLabel,
						Text = "Everything is fine 😎"
					}
				}
			};

			_stackContent.Children.Add(BuildClearButton());
			CreateContentPage().Content = _stackContent;

			CurrentItem = Items.Last();

			AddBottomTab("bottom 1");
			AddBottomTab("bottom 2");
			Shell.SetBackgroundColor(this, Color.BlueViolet);
		}

		Button BuildClearButton()
		{
			return new Button()
			{
				Text = "Click to clear ShellItem.Items",
				Command = new Command(() =>
				{
					try
					{
						Items.Last().Items.Clear();
					}
					catch (NotSupportedException)
					{
						Items.Clear();
						CreateContentPage().Content = _stackContent;
						((Label)_stackContent.Children[0]).Text = ExceptionMessage;
						CurrentItem = Items.Last();
					}
				}),
				AutomationId = ClearShellItems
			};
		}

#if UITEST
		[Test]
		public void ShellItemItemsClearTests()
		{
			RunningApp.WaitForElement(StatusLabel);
			RunningApp.Tap(ClearShellItems);

			var label = RunningApp.WaitForElement(StatusLabel)[0];
			Assert.AreEqual(label.Text, ExceptionMessage);
		}
#endif
	}
}
