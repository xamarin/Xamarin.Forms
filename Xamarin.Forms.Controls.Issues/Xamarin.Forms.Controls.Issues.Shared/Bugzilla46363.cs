using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 946363, "TapGestureRecognizer blocks List View Context Actions", PlatformAffected.Android)]
	public class Bugzilla46363 : TestContentPage 
	{
		protected override void Init()
		{
			var list = new List<string> { "one", "two", "three" };

			var lv = new ListView
			{
				ItemsSource = list,
				ItemTemplate = new DataTemplate(typeof(_46363Template))
			};

			Content = lv;
		}

		[Preserve(AllMembers = true)]
		class _46363Template : ViewCell
		{
			public _46363Template()
			{
				var label = new Label();
				label.SetBinding(Label.TextProperty, ".");
				View = new StackLayout { Children = { label } };

				ContextActions.Add(new MenuItem
				{
					Text = "Context Action",
					Command = new Command(() => { Debug.WriteLine($">>>>> _46363Template _46363Template 39: Context Action"); })
				});

				//View.GestureRecognizers.Add(new TapGestureRecognizer
				//{ Command = new Command(() => { Debug.WriteLine($">>>>> _46363Template _46363Template 47: Tap Gesture"); }) });
			}
		}

#if UITEST
		//[Test]
		//public void _46363Test()
		//{
		//	//RunningApp.WaitForElement(q => q.Marked(""));
		//}
#endif
	}
}