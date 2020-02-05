using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	public partial class Issue8881 : TestShell
	{
		public Issue8881()
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