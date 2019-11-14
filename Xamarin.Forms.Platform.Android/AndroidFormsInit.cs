using System;
using Android.Content;
using Android.OS;

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