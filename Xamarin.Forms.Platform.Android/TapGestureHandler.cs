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

			var result = false;

			var spanGestureRecognizers = SpanTapGestureRecognizers(count);
			foreach (var span in spanGestureRecognizers.Where(x=>x.Value.Count() > 0))
			{
				for(int i = 0; i < span.Key.Positions.Count; i++)
					if (span.Key.Positions[i].Contains(point.X, point.Y))
					{
						foreach (var recognizer in span.Value)
						{
							recognizer.SendTapped(view);
							result = true;
						}
					}
			}

			if (result)
				return result;

			IEnumerable<TapGestureRecognizer> gestureRecognizers = TapGestureRecognizers(count);
			foreach (TapGestureRecognizer gestureRecognizer in gestureRecognizers)
			{
				gestureRecognizer.SendTapped(view);
				result = true;
			}

			return result;
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

			return view.GestureRecognizers.GetGesturesFor<TapGestureRecognizer>(recognizer => recognizer.NumberOfTapsRequired == count);
		}

		public IDictionary<Span, IEnumerable<TapGestureRecognizer>> SpanTapGestureRecognizers(int count)
		{
			var spanRecognizers = new Dictionary<Span, IEnumerable<TapGestureRecognizer>>();

			foreach (var span in GetSpans())
				spanRecognizers.Add(span, span.GestureRecognizers.GetGesturesFor<TapGestureRecognizer>(recognizer => recognizer.NumberOfTapsRequired == count));

			return spanRecognizers;
		}
	}
}