using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public class FormsCommandBar : CommandBar
	{
		// TODO Once 10.0.14393.0 is available, enable dynamic overflow: https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.commandbar.isdynamicoverflowenabled.aspx 

		Windows.UI.Xaml.Controls.Button _moreButton;

		public FormsCommandBar()
		{
			PrimaryCommands.VectorChanged += OnCommandsChanged;
			SecondaryCommands.VectorChanged += OnCommandsChanged;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			_moreButton = GetTemplateChild("MoreButton") as Windows.UI.Xaml.Controls.Button;
			UpdateMore();
		}

		void OnCommandsChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs args)
		{
			UpdateMore();
		}

		void UpdateMore()
		{
			// TODO Add a version check; in 10.0.14393.0 and above we can just let the SDK handle this for us: https://msdn.microsoft.com/library/windows/apps/windows.ui.xaml.controls.commandbar.overflowbuttonvisibility.aspx
			if (_moreButton == null)
				return;

			_moreButton.Visibility = PrimaryCommands.Count > 0 || SecondaryCommands.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}