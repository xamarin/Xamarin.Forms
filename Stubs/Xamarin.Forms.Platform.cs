using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

#if __ANDROID__
using Xamarin.Forms.Platform.Android;
#elif TIZEN4_0
using Xamarin.Forms.Platform.Tizen;
#elif __IOS__
using Xamarin.Forms.Platform.iOS;
#endif

namespace Xamarin.Forms.Platform
{
	internal static class Loader
	{
		internal static void Load ()
		{
		}
	}

#if !WINDOWS_PHONE && !WINDOWS_PHONE_APP && !TIZEN4_0
	[RenderWith(typeof(BoxRenderer))]
#else
	[RenderWith (typeof(BoxViewRenderer))]
#endif
	internal class _BoxViewRenderer { }

	[RenderWith(typeof(EntryRenderer))]
	internal class _EntryRenderer { }

	[RenderWith (typeof (EditorRenderer))]
	internal class _EditorRenderer { }
#if __ANDROID__
	[RenderWith(typeof(Xamarin.Forms.Platform.Android.LabelRenderer))]
#else
	[RenderWith (typeof (LabelRenderer))]
#endif
	internal class _LabelRenderer { }

#if __ANDROID__
	[RenderWith(typeof(Xamarin.Forms.Platform.Android.ImageRenderer))]
#else
	[RenderWith (typeof (ImageRenderer))]
#endif
	internal class _ImageRenderer { }

	[RenderWith (typeof (ButtonRenderer))]
	internal class _ButtonRenderer { }

#if __ANDROID__
	[RenderWith(typeof(ImageButtonRenderer))]
#elif !TIZEN4_0
	[RenderWith(typeof(ImageButtonRenderer))]
#endif
	internal class _ImageButtonRenderer { }

	[RenderWith (typeof (TableViewRenderer))]
	internal class _TableViewRenderer { }

	[RenderWith (typeof (ListViewRenderer))]
	internal class _ListViewRenderer { }
#if !TIZEN4_0	
	[RenderWith (typeof (CollectionViewRenderer))]
	internal class _CollectionViewRenderer { }
	[RenderWith (typeof (CarouselViewRenderer))]
	internal class _CarouselViewRenderer { }
#endif
	[RenderWith (typeof (SliderRenderer))]
	internal class _SliderRenderer { }

	[RenderWith (typeof (WebViewRenderer))]
	internal class _WebViewRenderer { }

	[RenderWith (typeof (SearchBarRenderer))]
	internal class _SearchBarRenderer { }

	[RenderWith (typeof (SwitchRenderer))]
	internal class _SwitchRenderer { }

	[RenderWith (typeof (DatePickerRenderer))]
	internal class _DatePickerRenderer { }

	[RenderWith (typeof (TimePickerRenderer))]
	internal class _TimePickerRenderer { }

	[RenderWith (typeof (PickerRenderer))]
	internal class _PickerRenderer { }

	[RenderWith (typeof (StepperRenderer))]
	internal class _StepperRenderer { }

	[RenderWith (typeof (ProgressBarRenderer))]
	internal class _ProgressBarRenderer { }

	[RenderWith (typeof (ScrollViewRenderer))]
	internal class _ScrollViewRenderer { }

	[RenderWith (typeof (ActivityIndicatorRenderer))]
	internal class _ActivityIndicatorRenderer { }

	[RenderWith (typeof (FrameRenderer))]
	internal class _FrameRenderer { }


#if !WINDOWS_PHONE && !WINDOWS_PHONE_APP && !TIZEN4_0
	[RenderWith (typeof (OpenGLViewRenderer))]
#else
	[RenderWith (null)]
#endif
	internal class _OpenGLViewRenderer { }

#if !WINDOWS_PHONE && !WINDOWS_PHONE_APP && !TIZEN4_0
	[RenderWith (typeof (TabbedRenderer))]
#else
	[RenderWith (typeof (TabbedPageRenderer))]
#endif
	internal class _TabbedPageRenderer { }

#if !WINDOWS_PHONE && !WINDOWS_PHONE_APP && !TIZEN4_0
	[RenderWith (typeof (NavigationRenderer))]
#else
	[RenderWith (typeof (NavigationPageRenderer))]
#endif
	internal class _NavigationPageRenderer { }

	[RenderWith (typeof (CarouselPageRenderer))]
	internal class _CarouselPageRenderer { }

	[RenderWith (typeof (PageRenderer))]
	internal class _PageRenderer { }

#if !__IOS__ && !TIZEN4_0
	[RenderWith (typeof (MasterDetailRenderer))]
#elif TIZEN4_0
	[RenderWith (typeof(MasterDetailPageRenderer))]
#else
	[RenderWith (typeof (PhoneMasterDetailRenderer))]
#endif
	internal class _MasterDetailPageRenderer { }
}





