using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CollectionView7277), typeof(_7277CustomRenderer))]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class _7277CustomRenderer : CollectionViewRenderer
	{
		protected override bool IsLayoutReversed { get; set; } = true;

		public _7277CustomRenderer(Context context) : base(context)
		{

		}
	}
}