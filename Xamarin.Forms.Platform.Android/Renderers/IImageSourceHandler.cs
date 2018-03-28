using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace Xamarin.Forms.Platform.Android
{
	public interface IImageSourceHandler : IRegisterable
	{
		Task<Bitmap> LoadImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken));
	}

	public interface IImageSourceHandlerEx : IImageSourceHandler
	{
		Task<AnimationDrawable> LoadImageAnimationAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken));
	}
}