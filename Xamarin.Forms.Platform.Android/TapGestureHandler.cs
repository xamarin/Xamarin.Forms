using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal class TapGestureHandler
	{
		public TapGestureHandler(Func<View> getView, Func<IList<Span>> getSpans)
		{
			GetView = getView;
			GetSpans = getSpans;
		}

		Func<IList<Span>> GetSpans { get; }
		Func<View> GetView { get; }

		public void OnSingleClick()
		{
			// only handle click if we don't have double tap registered
			if (TapGestureRecognizers(2).Any())
				return;

			OnTap(1, new Point(-1, -1));
		}

		public bool OnTap(int count, Point point)
		{
			View view = GetView();

			if (view == null)
				return false;

			var captured = false;
			
			var overrides = view.ChildElementOverrides(point);
			for (int i = 0; i < overrides.Count; i++)
				foreach (TapGestureRecognizer recognizer in overrides[i].GestureRecognizers)
				{
					recognizer.SendTapped(view);
					captured = true;
				}

			if (captured)
				return captured;

			IEnumerable<TapGestureRecognizer> gestureRecognizers = TapGestureRecognizers(count);
			foreach (TapGestureRecognizer gestureRecognizer in gestureRecognizers)
			{
				gestureRecognizer.SendTapped(view);
				captured = true;
			}

			return captured;
		}

		public bool HasAnyGestures()
		{
			var view = GetView();
			return view != null && view.GestureRecognizers.OfType<TapGestureRecognizer>().Any();
		}

		public IEnumerable<TapGestureRecognizer> TapGestureRecognizers(int count)
		{
			View view = GetView();
			if (view == null)
				return Enumerable.Empty<TapGestureRecognizer>();

			return view.GestureRecognizers.GetGesturesFor<TapGestureRecognizer>(recognizer => recognizer.NumberOfTapsRequired == count).ToList();

		}

	}
}