using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7758, "(iOS) EmptyView is not rendered in center", PlatformAffected.iOS)]
	public partial class Issue7758 : TestContentPage
	{
		public Issue7758()
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