#if __MOBILE__
using NativeImageView = UIKit.UIImageView;
using NativeImage = UIKit.UIImage;
namespace Xamarin.Forms.Platform.iOS
#else
using NativeImageView = AppKit.NSImageView;
using NativeImage = AppKit.NSImage;
using CoreAnimation;

namespace Xamarin.Forms.Platform.MacOS
#endif
{
#if __MACOS__
	public interface IImageView
	{
		NativeImage Image { get; set; }

		CALayer Layer { get; set; } 
	}
#endif

	public interface IImageVisualElementRenderer : IVisualNativeElementRenderer
	{
		void SetImage(NativeImage image);
		bool IsDisposed { get; }
		IImageView GetImage();
	}
}