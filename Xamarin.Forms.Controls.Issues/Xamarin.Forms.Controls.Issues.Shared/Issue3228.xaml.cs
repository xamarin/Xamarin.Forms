using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3228, "Default Search Directory for UWP Icons", PlatformAffected.UWP)]
	public partial class Issue3228 : TestContentPage
	{
		protected override void Init()
		{
		}

		public Issue3228()
		{
#if APP
			InitializeComponent();
			On<Windows>().SetImageDirectory(text1.Text);
			image4.ClearValue(PlatformConfiguration.WindowsSpecific.Image.ImageDirectoryProperty);
			text1.TextChanged += (_, __) => On<Windows>().SetImageDirectory(text1.Text); // all images on page
			text2.TextChanged += (_, __) => image3.On<Windows>().SetImageDirectory(text2.Text); // only image3
#endif
		}

		void Button_Clicked(object sender, System.EventArgs e)
			=> layout1.Children.Add(new Image { Source = "Logo.png" });

		void OnClean(object sender, System.EventArgs e)
			=> ClearValue(PlatformConfiguration.WindowsSpecific.Page.ImageDirectoryProperty);
	}
}