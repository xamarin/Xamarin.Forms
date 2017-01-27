using Android.App;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls.Issues;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(Bugzilla38989._38989CustomViewCell), typeof(_38989CustomViewCellRenderer))]

namespace Xamarin.Forms.ControlGallery.Android
{
	public class _38989CustomViewCellRenderer : Platform.Android.ViewCellRenderer
	{
		protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
		{
			AView nativeView = convertView;

			if (nativeView == null)
				nativeView = (context as Activity).LayoutInflater.Inflate(Resource.Layout.Layout38989, null);

			return nativeView;
		}
	}
}