using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections;
using System.Linq;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8263, "[Enhancement] Add On/Off VisualStates for Switch")]
	public partial class Issue8263 : TestContentPage
    {
        public Issue8263()
        {
#if APP
			InitializeComponent();
#endif
		}
		protected override void Init()
		{

		}

#if UITEST
		[Test]
		public void SwitchOnOffVisualStatesTest()
		{
			RunningApp.WaitForElement("Switch");
			RunningApp.Screenshot("Switch Default");
			RunningApp.Tap("Switch");
			RunningApp.Screenshot("Switch Off with Red ThumbColor");
			RunningApp.Tap("Switch");
			RunningApp.Screenshot("Switch On with Green ThumbColor");
		}
#endif
	}
}