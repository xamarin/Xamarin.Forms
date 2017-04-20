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
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(_50787CustomRenderer))]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class _50787CustomRenderer : NavigationPageRenderer
	{
		protected override int TransitionDuration { get; set; } = 500;

		protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
		{
			if (isPush)
				transaction.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_left);
			else
				transaction.SetCustomAnimations(Resource.Animation.enter_from_left, Resource.Animation.exit_to_right);
		}
	}
}