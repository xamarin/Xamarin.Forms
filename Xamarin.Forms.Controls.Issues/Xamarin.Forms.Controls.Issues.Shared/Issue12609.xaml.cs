using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 12609, "Shapes.Path render erro", PlatformAffected.All)]
	public partial class Issue12609 : TestContentPage
	{
		public Issue12609()
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