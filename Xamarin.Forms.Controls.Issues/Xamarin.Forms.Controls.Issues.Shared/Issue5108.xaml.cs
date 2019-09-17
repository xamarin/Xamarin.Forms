using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using System;

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
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5108, "iOS: Frame with HasShadow set to true and BackgroundColor alpha < 1 casts shadow on all child views", PlatformAffected.iOS)]
	public partial class Issue5108 : TestContentPage
	{
		public Issue5108()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}


		void MarginButton_Clicked(object sender, EventArgs e)
		{
			if (myframe.Margin.Top == 20)
				myframe.Margin = new Thickness(5);
			else
				myframe.Margin = new Thickness(20);

		}

		void HasShadowButton_Clicked(object sender, EventArgs e)
		{
			myframe.HasShadow = !myframe.HasShadow;
		}

		void RadiusButton_Clicked(object sender, EventArgs e)
		{
			if (myframe.CornerRadius == 10)
				myframe.CornerRadius = 20;
			else
				myframe.CornerRadius = 10;
		}

		Color? initialColor = null;
		void BackgroundButton_Clicked(object sender, EventArgs e)
		{
			if (!initialColor.HasValue)
				initialColor = myframe.BackgroundColor;

			if (myframe.BackgroundColor == initialColor.Value)
				myframe.BackgroundColor = Color.HotPink;
			else
				myframe.BackgroundColor = initialColor.Value;
		}
	}
}
