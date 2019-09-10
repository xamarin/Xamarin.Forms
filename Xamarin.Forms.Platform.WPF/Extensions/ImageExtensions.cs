using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Xamarin.Forms.Internals;
using WImageSource = System.Windows.Media.ImageSource;

namespace Xamarin.Forms.Platform.WPF
{
	/// <summary>Helper to convert xamarin image stuff to wpf image stuff.</summary>
	public static class ImageExtensions
	{
		/// <summary>Convert xamarin stretch to wpf aspect.</summary>
		/// <param name="aspect"></param>
		/// <returns></returns>
		public static Stretch ToStretch(this Aspect aspect)
		{
			switch (aspect)
			{
				case Aspect.Fill:
					return Stretch.Fill;
				case Aspect.AspectFill:
					return Stretch.UniformToFill;
				default:
				case Aspect.AspectFit:
					return Stretch.Uniform;
			}
		}

		/// <summary>Convert a xamarin image source to a wpf image source.</summary>
		/// <param name="source">Xamarin forms image source</param>
		/// <param name="cancellationToken">Cancellation token for the operation.</param>
		/// <returns>Wpf image source</returns>
		public static async Task<WImageSource> ToWindowsImageSourceAsync(this ImageSource source, CancellationToken cancellationToken = default)
		{
			if (source == null || source.IsEmpty)
				return null;

			var handler = Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source);
			if (handler == null)
				return null;

			try
			{
				return await handler.LoadImageAsync(source, cancellationToken);
			}
			catch (OperationCanceledException)
			{
				Log.Warning("Image loading", "Image load cancelled");
			}
			catch (Exception ex)
			{
				Log.Warning("Image loading", $"Image load failed: {ex}");
			}

			return null;
		}
	}
}
