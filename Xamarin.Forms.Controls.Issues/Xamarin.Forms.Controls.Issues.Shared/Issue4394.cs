using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4394, "ListView cannot reset selected status", PlatformAffected.iOS)]
	public class Issue4394 : ContentPage
	{
		public Issue4394()
		{
			var sampleList = new ListView()
			{
				ItemsSource = new List<string>()
				{
					"Test", "Demo", "Sample"
				}
			};

			Content = sampleList;
		}
	}
}