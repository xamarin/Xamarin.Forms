using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Xamarin.Forms.Platform.WPF.CustomControls
{
	class TapItem
	{
		public FrameworkElement FrameworkElement { get; set; }
		public Action<object, TapEventArgs> TapAction { get; set; }
		public Action<object, TapEventArgs> DoubleTapAction { get; set; }
	}
}
