using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android.Renderers;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(Bugzilla42534EntryRenderer))]
namespace Xamarin.Forms.ControlGallery.Android.Renderers
{
	public class Bugzilla42534EntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			//if(e.OldElement != null)
			//{
			//    Control.SetOnEditorActionListener(this);
			//    Control.AddTextChangedListener(this);
			//    SetNativeControl(Control);
			//}

			if (Control != null)
			{
				var nativeEditText = Control as EditText;

				nativeEditText.SetSelectAllOnFocus(true);
			}
		}
	}
}