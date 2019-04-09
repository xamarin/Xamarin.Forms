﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

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
	[Issue(IssueTracker.Github, 2, "Issue Description (Markup)", PlatformAffected.Default)]
	public partial class _TemplateMarkup : TestContentPage
	{
		public _TemplateMarkup()
		{
#if APP
			InitializeComponent();

			BindingContext = new ViewModelGithub2();
#endif
		}

		protected override void Init()
		{

		}

#if UITEST
		[Test]
		public void Github2Test()
		{
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot("I am at Github 2");
			RunningApp.WaitForElement(q => q.Marked("Github2Label"));
			RunningApp.Screenshot("I see the Label");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class ViewModelGithub2
	{
		public ViewModelGithub2()
		{
			
		}
	}

	[Preserve(AllMembers = true)]
	public class ModelGithub2
	{
		public ModelGithub2()
		{
			
		}
	}
}