using Android.Support.V7.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal class RecyclerExtensions
	{
		public static int CalculateCenterItemIndex(int firstVisibleItemIndex, RecyclerView recyclerView, LinearLayoutManager linearLayoutManager)
		{
			// This can happen if a layout pass has not happened yet
			if (firstVisibleItemIndex == -1)
				return firstVisibleItemIndex;

			AView centerView;

			if (linearLayoutManager.Orientation == LinearLayoutManager.Horizontal)
			{
				float centerX = recyclerView.Width / 2;
				centerView = recyclerView.FindChildViewUnder(centerX, recyclerView.Top);
			}
			else
			{
				float centerY = recyclerView.Height / 2;
				centerView = recyclerView.FindChildViewUnder(recyclerView.Left, centerY);
			}

			if (centerView != null)
				return recyclerView.GetChildAdapterPosition(centerView);

			return firstVisibleItemIndex;
		}
	}
}