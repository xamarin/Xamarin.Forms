using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Linq;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.UITest.iOS;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7792, "(Android) CarouselView string EmptyView not displayed", PlatformAffected.Android)]
	public partial class Issue7792 : TestContentPage
    {
        public Issue7792()
        {
			Title = "Issue 7792";

			BindingContext = new Issue7792ViewModel();
		}

		protected override void Init()
		{
			var layout = new StackLayout();

			var instructions = new Label
			{
				Text = "If you can see the text of the Carousel EmptyView below, the test passes."
			};

			var carousel = new CollectionView
			{
				BackgroundColor = Color.Yellow,
				EmptyView = "No items to display (EmptyView)."
			};

			carousel.SetBinding(ItemsView.ItemsSourceProperty, "EmptyItems");

			layout.Children.Add(instructions);
			layout.Children.Add(carousel);

			Content = layout;
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue7792Model
	{
		public string Text1 { get; set; }
		public string Text2 { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Issue7792ViewModel : BindableObject
	{	
  		public IList<Issue7792Model> EmptyItems { get; private set; }
	}
}