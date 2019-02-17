using System.ComponentModel;
using AViews = Android.Views;
using AWidget = Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(ContentDescriptionEffectRenderer), ContentDescriptionEffect.EffectName)]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class ContentDescriptionEffectRenderer : PlatformEffect
	{

		protected override void OnAttached()
		{
		}

		protected override void OnDetached()
		{
		}

		protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
		{
			var button = Element as Button;
			var viewGroup = Control as AViews.ViewGroup;
			var nativeButton = Control as AWidget.Button;

			if (nativeButton != null && viewGroup != null && viewGroup.ChildCount > 0)
			{
				nativeButton = viewGroup.GetChildAt(0) as AWidget.Button;
			}

			if (button == null || nativeButton == null)
			{
				return;
			}

			button.SetValue(
				ContentDescriptionEffectProperties.ContentDescriptionProperty,
				nativeButton.ContentDescription);
		}

	}
}
