using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
	[Category(UITestCategories.Shape)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12339, "[Bug] LinearGradientBrush doesn't apply Binding-Colors", PlatformAffected.All)]
	public partial class Issue12339 : TestContentPage
	{
		public Issue12339()
		{
#if APP
			InitializeComponent();
			BindingContext = this;
#endif
		}

		public Color Start { get; set; } = Color.Red;
		public Color Stop { get; set; } = Color.Orange;

		protected override void Init()
		{

		}
	}
}