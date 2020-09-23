using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using AndroidX.Legacy.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms.Internals;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class Platform
	{
		public static IVisualElementRenderer CreateRendererWithContext(VisualElement element, Context context)
		{
			return AppCompat.Platform.CreateRendererWithContext(element, context);
		}
	}
}