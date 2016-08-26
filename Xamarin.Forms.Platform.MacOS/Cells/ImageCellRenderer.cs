using System.ComponentModel;
using System.Threading.Tasks;
using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ImageCellRenderer : TextCellRenderer
	{
		public override NSView GetCell(Cell item, NSView reusableView, NSTableView tv)
		{
			var tvc = reusableView as CellNSView;
			if (tvc == null)
				tvc = new CellNSView(NSTableViewCellStyle.ImageSubtitle);

			var result = (CellNSView)base.GetCell(item, tvc, tv);

			var imageCell = (ImageCell)item;

			WireUpForceUpdateSizeRequested(item, result, tv);

			SetImage(imageCell, result);

			return result;
		}

		protected override void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			var tvc = (CellNSView)sender;
			var imageCell = (ImageCell)tvc.Cell;

			base.HandlePropertyChanged(sender, args);

			if (args.PropertyName == ImageCell.ImageSourceProperty.PropertyName)
				SetImage(imageCell, tvc);
		}

		static async void SetImage(ImageCell cell, CellNSView target)
		{
			var source = cell.ImageSource;

			target.ImageView.Image = null;

			IImageSourceHandler handler;

			if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
			{
				NSImage uiimage;
				try
				{
					uiimage = await handler.LoadImageAsync(source).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					uiimage = null;
				}

				NSRunLoop.Main.BeginInvokeOnMainThread(() =>
				{
					target.ImageView.Image = uiimage;
					target.NeedsLayout = true;
				});
			}
			else
				target.ImageView.Image = null;
		}
	}
}

