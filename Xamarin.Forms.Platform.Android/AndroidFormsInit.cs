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

namespace Xamarin.Forms.Platform.Android
{
	public class AndroidFormsInit : FormsInit
	{
		public AndroidFormsInit(Action init) : base(init)
		{
		}

		public Context Activity { get; set; } 
		public Bundle Bundle { get; set; }
	}
}