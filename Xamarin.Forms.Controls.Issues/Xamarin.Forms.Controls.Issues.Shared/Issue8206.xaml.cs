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
	[Issue(IssueTracker.Github, 8206, "FlyoutBackgroundImage is not displayed in UWP", PlatformAffected.UWP)]
	public partial class Issue8206 : Shell
    {
        public Issue8206()
        {
#if APP
			InitializeComponent();
#endif 
        }
    }
}