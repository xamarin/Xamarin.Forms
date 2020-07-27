using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6220, "Label Visibility on Shell", PlatformAffected.Android)]
	public partial class Issue6220 : TestShell
	{
		public Issue6220()
		{
#if APP
			this.InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}
	}
}