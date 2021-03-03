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
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.ControlGallery.Android;
using Microsoft.Maui.Controls.ControlGallery.Issues;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(Bugzilla38989._38989CustomViewCell), typeof(_38989CustomViewCellRenderer))]

namespace Microsoft.Maui.Controls.ControlGallery.Android
{
	public class _38989CustomViewCellRenderer : Microsoft.Maui.Controls.Compatibility.Platform.Android.ViewCellRenderer
	{
		protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
		{
			var nativeView = convertView;

			if (nativeView == null)
				nativeView = (context.GetActivity()).LayoutInflater.Inflate(Resource.Layout.Layout38989, null);

			return nativeView;
		}
	}
}