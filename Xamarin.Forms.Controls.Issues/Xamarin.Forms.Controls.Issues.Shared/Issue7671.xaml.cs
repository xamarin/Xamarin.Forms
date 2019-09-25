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
	[Issue(IssueTracker.None, 7671, "Check ControlTemplate rendering", PlatformAffected.WPF)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Issue7671 : ContentPage
	{
		public Issue7671()
		{
			InitializeComponent();
		}
	}
}