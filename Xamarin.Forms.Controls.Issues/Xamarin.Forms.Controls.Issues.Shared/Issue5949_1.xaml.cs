using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[Preserve(AllMembers = true)]
	public partial class Issue5949_1 : MasterDetailPage
	{
		public Issue5949_1()
		{
			InitializeComponent();
		}
	}
}