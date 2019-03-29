using System.ComponentModel;
using AViews = Android.Views;
using AWidget = Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Platform.Android;
using Android.Support.V4.View;
using Android.Support.V4.View.Accessibility;
using Android.AccessibilityServices;

[assembly: ExportEffect(typeof(ContentDescriptionEffectRenderer), ContentDescriptionEffect.EffectName)]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class ContentDescriptionEffectRenderer : PlatformEffect
	{

		protected override void OnAttached()
		{
			Element.PropertyChanged += Element_PropertyChanged;
		}

		void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Element_PropertyChanged " + e.PropertyName);
		}


		protected override void OnDetached()
		{
		}

		protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine("OnElementPropertyChanged" + args.PropertyName);


			var button = Element as Button;
			var renderer = Platform.Android.Platform.GetRenderer(button);
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

			var info = AccessibilityNodeInfoCompat.Obtain(nativeButton);

			var hasDelegate = ViewCompat.HasAccessibilityDelegate(nativeButton);
			ViewCompat.OnInitializeAccessibilityNodeInfo(nativeButton, info);

			System.Diagnostics.Debug.WriteLine(info.ContentDescription);
			System.Diagnostics.Debug.WriteLine(nativeButton.ContentDescription);

			button.SetValue(
				ContentDescriptionEffectProperties.NameAndHelpTextProperty,
				info.ContentDescription);

			button.SetValue(
				ContentDescriptionEffectProperties.ContentDescriptionProperty,
				nativeButton.ContentDescription);
		}

	}
}
