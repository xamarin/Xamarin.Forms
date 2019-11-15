using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6187, "Visual Material Entry Underline color should reference PlaceholderColor",
		PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Entry)]
#endif
	public class Issue6187 : TestContentPage
	{
		protected override void Init()
		{
			Visual = VisualMarker.Material;

			Entry entry = new Entry
			{
				Placeholder = "Enter text...",
				PlaceholderColor = Color.Green,
				TextColor = Color.Red
			};

			StackLayout layout = new StackLayout
			{
				Padding = new Thickness(0, 50, 0, 0)
			};

			layout.Children.Add(entry);

			Content = layout;
		}



#if UITEST
		[Test]
		public void ImageShouldLayoutOnTopOfButton()
		{
			
		}
#endif
	}
}
