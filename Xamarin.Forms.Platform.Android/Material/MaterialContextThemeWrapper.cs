using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android.Material
{
	public class MaterialContextThemeWrapper : ContextThemeWrapper
	{
		public MaterialContextThemeWrapper(Context @base) : base(@base, Resource.Style.XamarinFormsMaterialTheme)
		{
		}


		public static Context Create(Context context)
		{
			if (context is MaterialContextThemeWrapper)
				return context;

			return new MaterialContextThemeWrapper(context);
		}
	}
}