﻿#if __ANDROID_29__
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
#else
using Android.Support.V7.Widget;
#endif
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal static class RecyclerExtensions
	{
		public static int CalculateCenterItemIndex(this RecyclerView recyclerView, int firstVisibleItemIndex, LinearLayoutManager linearLayoutManager, bool lookCenteredOnXAndY)
		{
			// This can happen if a layout pass has not happened yet
			if (firstVisibleItemIndex == -1)
				return firstVisibleItemIndex;

			AView centerView;

			if (linearLayoutManager.Orientation == LinearLayoutManager.Horizontal)
			{
				float centerX = recyclerView.Width / 2;
				float centerY = recyclerView.Top;

				if (lookCenteredOnXAndY)
					centerY = recyclerView.Height / 2;

				centerView = recyclerView.FindChildViewUnder(centerX, centerY);
			}
			else
			{
				float centerY = recyclerView.Height / 2;
				float centerX = recyclerView.Left;

				if (lookCenteredOnXAndY)
					centerX = recyclerView.Width / 2;

				centerView = recyclerView.FindChildViewUnder(centerX, centerY);
			}

			if (centerView != null)
				return recyclerView.GetChildAdapterPosition(centerView);

			return firstVisibleItemIndex;
		}
	}
}