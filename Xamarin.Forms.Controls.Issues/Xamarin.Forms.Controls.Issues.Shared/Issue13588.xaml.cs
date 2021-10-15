using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CollectionView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13588, "[Bug] Button inside collectionview not appearing until UWP window is resized #13552", PlatformAffected.UWP)]
	public partial class Issue13588 : TestContentPage
	{
		public Issue13588()
		{
#if APP
			Title = "Issue 13588";
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}
	}
}