using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.Markup
{
	public static class ElementGesturesExtensions
	{
		const string bindingContextPath = Binding.SelfPath;

		public static TGestureElement BindClickGesture<TGestureElement>(
			this TGestureElement gestureElement,
			string path = bindingContextPath
		) where TGestureElement : Element, IGestureRecognizers
			=> ClickGesture(gestureElement, g => g.Bind(path));

		public static TGestureElement BindSwipeGesture<TGestureElement>(
			this TGestureElement gestureElement,
			string path = bindingContextPath
		) where TGestureElement : Element, IGestureRecognizers
			=> SwipeGesture(gestureElement, g => g.Bind(path));

		public static TGestureElement BindTapGesture<TGestureElement>(
			this TGestureElement gestureElement,
			string path = bindingContextPath
		) where TGestureElement : Element, IGestureRecognizers
			=> TapGesture(gestureElement, g => g.Bind(path));

		public static TGestureElement ClickGesture<TGestureElement>(this TGestureElement gestureElement, Action<ClickGestureRecognizer> init)
			where TGestureElement : Element, IGestureRecognizers
			=> Gesture(gestureElement, init);

		public static TGestureElement PanGesture<TGestureElement>(this TGestureElement gestureElement, Action<PanGestureRecognizer> init)
			where TGestureElement : Element, IGestureRecognizers
			=> Gesture(gestureElement, init);

		public static TGestureElement PinchGesture<TGestureElement>(this TGestureElement gestureElement, Action<PinchGestureRecognizer> init)
			where TGestureElement : Element, IGestureRecognizers
			=> Gesture(gestureElement, init);

		public static TGestureElement SwipeGesture<TGestureElement>(this TGestureElement gestureElement, Action<SwipeGestureRecognizer> init)
			where TGestureElement : Element, IGestureRecognizers
			=> Gesture(gestureElement, init);

		public static TGestureElement TapGesture<TGestureElement>(this TGestureElement gestureElement, Action<TapGestureRecognizer> init)
			where TGestureElement : Element, IGestureRecognizers
			=> Gesture(gestureElement, init);

		/// <summary>
		/// Ensures that <typeparamref name="TGestureElement"/> has a <typeparamref name="TGestureRecognizer"/>,
		/// and passes it to the supplied <paramref name="init"/> Action
		/// </summary>
		public static TGestureElement Gesture<TGestureElement, TGestureRecognizer>(
			this TGestureElement gestureElement,
			Action<TGestureRecognizer> init
		) where TGestureElement : Element, IGestureRecognizers
		  where TGestureRecognizer : GestureRecognizer, new()
		{
			init.Invoke(Ensure<TGestureRecognizer>(gestureElement.GestureRecognizers));
			return gestureElement;
		}

		static TGestureRecognizer Ensure<TGestureRecognizer>(IList<IGestureRecognizer> gestureRecognizers)
			where TGestureRecognizer : GestureRecognizer, new()
		{
			var gestureRecognizer = (TGestureRecognizer)gestureRecognizers.FirstOrDefault(g => g is TGestureRecognizer);

			if (gestureRecognizer == null)
				gestureRecognizers.Add(gestureRecognizer = new TGestureRecognizer());

			return gestureRecognizer;
		}
	}
}
