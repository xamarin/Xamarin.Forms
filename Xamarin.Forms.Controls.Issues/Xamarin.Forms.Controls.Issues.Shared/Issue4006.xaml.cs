using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4006, "OnIdiom default value")]
	public partial class Issue4006 : TestContentPage
	{
		public Issue4006()
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