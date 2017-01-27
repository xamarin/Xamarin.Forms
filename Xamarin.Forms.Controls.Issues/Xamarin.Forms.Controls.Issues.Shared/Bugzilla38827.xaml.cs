namespace Xamarin.Forms.Controls.Issues
{
	public partial class Bugzilla38827 : ContentPage
	{
		public Bugzilla38827()
		{
#if !UITEST
			InitializeComponent();
#endif
		}
	}
}