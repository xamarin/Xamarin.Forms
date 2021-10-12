using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9079,
		"[Bug] [UWP] CollectionView ItemsLayout not working when initially invisible",
		PlatformAffected.UWP)]
	public partial class Issue9079 : TestContentPage
	{
		public Issue9079()
		{
#if APP
			InitializeComponent();
			GenerateItems();
#endif
		}

		protected override void Init()
		{

		}
#if APP
		void GenerateItems()
		{
			List<string> items = new List<string>();
			for (int i = 0; i < 100; i++)
				items.Add($"Hello world {i}!");

			collectionView.ItemsSource = items;
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			collectionView.IsVisible = !collectionView.IsVisible;
		}
#endif
	}
}