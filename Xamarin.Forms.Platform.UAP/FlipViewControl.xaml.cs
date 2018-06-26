using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed partial class FlipViewControl : UserControl
	{
		public bool IsSwipeEnabled;

		public FlipViewControl(bool isSwipeEnabled)
		{
			InitializeComponent();
			IsSwipeEnabled = isSwipeEnabled;
		}

		void VirtualizingStackPanel_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			e.Handled = !IsSwipeEnabled;
		}
	}
}
