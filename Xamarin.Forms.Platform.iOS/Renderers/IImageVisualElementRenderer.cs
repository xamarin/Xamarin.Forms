#if __MOBILE__
using ImageNativeView = UIKit.UIImageView;
sing NativeImage = UIKit.UIImage;
namespace Xamarin.Forms.Platform.iOS
#else
using ImageNativeView = AppKit.NSImageView;
using NativeImage = AppKit.NSImage;
namespace Xamarin.Forms.Platform.MacOS
#endif
{
	public interface IImageVisualElementRenderer : IVisualNativeElementRenderer
	{
		void SetImage(NativeImage image);
		bool IsDisposed { get; }
		ImageNativeView GetImage();
	}
}