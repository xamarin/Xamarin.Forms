using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shape)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11349,
		"[Bug] SwipeView stopped working using a RefreshView",
		PlatformAffected.Android)]
	public partial class Issue11349 : TestContentPage
	{
		public Issue11349()
		{
#if APP
			Device.SetFlags(new List<string> { ExperimentalFlags.ExpanderExperimental, ExperimentalFlags.SwipeViewExperimental });
			InitializeComponent();
			BindingContext = this;
#endif
		}

		public ObservableCollection<string> Items { get; set; }

		protected override void Init()
		{
			Items = new ObservableCollection<string>();

			for (int i = 0; i < 10; i++)
				Items.Add($"Item {i + 1}");
		}

#if APP
		void OnSwipeItemViewInvoked(object sender, EventArgs e)
		{
			DisplayAlert("SwipeView", "SwipeItemView Invoked", "Ok");
		}

		void OnExpanderTapped(object sender, EventArgs e)
		{
			DisplayAlert("Expander", "Expander Tapped", "Ok");
		}
#endif
	}
}