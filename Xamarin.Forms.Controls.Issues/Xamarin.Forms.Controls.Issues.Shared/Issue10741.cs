using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10741, "[IOS Andriod] Date Picker does not have a place holder ", PlatformAffected.Android)]
	public class Issue10741 : TestContentPage
	{

		const string DatePicker = "DatePicker";
		
		protected override void Init()
		{

			var datePicker = new DatePicker
			{
				AutomationId = DatePicker
				
			};
			datePicker.Placeholder = "Test";
			datePicker.PlaceHolderColor = Color.LightGray;



			Content = new StackLayout
			{
				Children =
				{

					datePicker
					}

			};


		}
	}
	}