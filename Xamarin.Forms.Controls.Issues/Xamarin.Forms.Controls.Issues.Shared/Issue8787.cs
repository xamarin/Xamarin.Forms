﻿using System;
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
	[Issue(IssueTracker.Github, 8787, "[Bug] Entry text initially invisible on UWP",
		PlatformAffected.UWP)]
	public partial class Issue8787 : TestContentPage
	{
		public Issue8787()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{			
		}
	}
}

