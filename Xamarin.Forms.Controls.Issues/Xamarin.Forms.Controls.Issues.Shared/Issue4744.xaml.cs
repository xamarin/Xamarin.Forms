﻿using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;

#if UITEST && __ANDROID__
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
using System.Linq;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Xamarin.Forms.Controls.Issues
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4744, "forms in UWP show empty button for DisplayActionSheet", PlatformAffected.UWP)]
	public partial class Issue4744 : TestContentPage
	{
#if APP
		public Issue4744()
		{
			InitializeComponent();
		}
#endif
		protected override void Init()
		{
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			DisplayActionSheet("Title", "Cancel", "Destruction", "button", string.Empty);
		}

		private void Button_Clicked_1(object sender, EventArgs e)
		{
			DisplayActionSheet("Title", "Cancel", "Destruction", "button", null);
		}

		private void Button_Clicked_2(object sender, EventArgs e)
		{
			DisplayActionSheet("Title", "Cancel", "Destruction", "button", "Hello");
		}
	}
}
