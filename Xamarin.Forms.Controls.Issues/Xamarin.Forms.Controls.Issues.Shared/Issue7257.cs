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
	[Issue(IssueTracker.Github, 7257, "[Bug] Scale/Translate works in different order between UWP and Android", PlatformAffected.UWP)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.SearchBar)]
#endif
	public class Issue7257 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 7257";

			StackLayout contentSL = new StackLayout();

			Label instructions = new Label
			{
				Text = "Look at the layout below.\n\n"
				     + "The green and the blue squares should both be at the same vertical position as the red bar to the left, and should be same size.\n\n"
					 + "If the green square is below the blue square, it means there is a problem.",
				Margin = 10
			};

			contentSL.Children.Add(instructions);

			AbsoluteLayout testAL = new AbsoluteLayout();

			// Reference red rectangle on left edge 100 from top, 50 high
			Xamarin.Forms.AbsoluteLayout referenceRectAL = new Xamarin.Forms.AbsoluteLayout();
			referenceRectAL.BackgroundColor = Xamarin.Forms.Color.Red;
			Xamarin.Forms.AbsoluteLayout.SetLayoutBounds(referenceRectAL, new Xamarin.Forms.Rectangle(0, 0, 10, 50));
			Xamarin.Forms.AbsoluteLayout.SetLayoutFlags(referenceRectAL, Xamarin.Forms.AbsoluteLayoutFlags.None);
			referenceRectAL.TranslationX = 0;
			referenceRectAL.TranslationY = 100;
			testAL.Children.Add(referenceRectAL);

			// Problem green rectangle positioned at 100, 100, size 100, 100, scaled 50%.
			Xamarin.Forms.AbsoluteLayout scaleRectAL = new Xamarin.Forms.AbsoluteLayout();
			scaleRectAL.BackgroundColor = Xamarin.Forms.Color.Green;
			Xamarin.Forms.AbsoluteLayout.SetLayoutBounds(scaleRectAL, new Xamarin.Forms.Rectangle(0, 0, 100, 100));
			Xamarin.Forms.AbsoluteLayout.SetLayoutFlags(scaleRectAL, Xamarin.Forms.AbsoluteLayoutFlags.None);
			scaleRectAL.AnchorX = 0;
			scaleRectAL.AnchorY = 0;
			scaleRectAL.ScaleX = 0.5;
			scaleRectAL.ScaleY = 0.5;
			scaleRectAL.TranslationX = 100;
			scaleRectAL.TranslationY = 100;
			testAL.Children.Add(scaleRectAL);

			// Blue rectangle positioned at 200, 100, size 100, 100, scaled 50% - rotated invisibly a small amount on X-axis.
			Xamarin.Forms.AbsoluteLayout scaleRect3DAL = new Xamarin.Forms.AbsoluteLayout();
			scaleRect3DAL.BackgroundColor = Xamarin.Forms.Color.Blue;
			Xamarin.Forms.AbsoluteLayout.SetLayoutBounds(scaleRect3DAL, new Xamarin.Forms.Rectangle(0, 0, 100, 100));
			Xamarin.Forms.AbsoluteLayout.SetLayoutFlags(scaleRect3DAL, Xamarin.Forms.AbsoluteLayoutFlags.None);
			scaleRect3DAL.AnchorX = 0;
			scaleRect3DAL.AnchorY = 0;
			scaleRect3DAL.ScaleX = 0.5;
			scaleRect3DAL.ScaleY = 0.5;
			scaleRect3DAL.TranslationX = 200;
			scaleRect3DAL.TranslationY = 100;
			scaleRect3DAL.RotationX = 0.001;  //rotate miniscule amount on X-axis, to force 3D projection.
			testAL.Children.Add(scaleRect3DAL);

			contentSL.Children.Add(testAL);
			Content = contentSL;
		}
	}
}
