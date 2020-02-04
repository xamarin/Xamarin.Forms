using System;

namespace Xamarin.Forms
{
	public class AdaptiveTrigger : StateTriggerBase
	{
		public AdaptiveTrigger()
		{
			UpdateState();

			if (!DesignMode.IsDesignModeEnabled)
			{
				var weakEvent = new WeakEventListener<AdaptiveTrigger, object, EventArgs>(this)
				{
					OnEventAction = (instance, source, eventArgs) => OnSizeChanged(source, eventArgs),
					OnDetachAction = (listener) => { Application.Current.MainPage.SizeChanged -= listener.OnEvent; }
				};

				Application.Current.MainPage.SizeChanged += weakEvent.OnEvent;
			}
		}

		public double MinWindowHeight
		{
			get => (double)GetValue(MinWindowHeightProperty);
			set => SetValue(MinWindowHeightProperty, value);
		}

		public static readonly BindableProperty MinWindowHeightProperty =
		BindableProperty.Create(nameof(MinWindowHeight), typeof(double), typeof(AdaptiveTrigger), default(double),
			propertyChanged: OnMinWindowHeightChanged);

		static void OnMinWindowHeightChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((AdaptiveTrigger)bindable).UpdateState();
		}

		public double MinWindowWidth
		{
			get => (double)GetValue(MinWindowWidthProperty);
			set => SetValue(MinWindowWidthProperty, value);
		}

		public static readonly BindableProperty MinWindowWidthProperty =
			BindableProperty.Create(nameof(MinWindowWidthProperty), typeof(double), typeof(AdaptiveTrigger), default(double),
				propertyChanged: OnMinWindowWidthChanged);

		static void OnMinWindowWidthChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((AdaptiveTrigger)bindable).UpdateState();
		}

		void OnSizeChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		void UpdateState()
		{
			var mainPage = Application.Current.MainPage;

			var w = mainPage.Width;
			var h = mainPage.Height;
			var mw = MinWindowWidth;
			var mh = MinWindowHeight;

			ResetActiveTriggers();
			SetActive(w >= mw && h >= mh);
		}

		void ResetActiveTriggers()
		{
			if (VisualState == null)
				return;

			var group = VisualState.VisualStateGroup;

			if (group == null)
				return;
			
			for (var stateIndex = 0; stateIndex < group.States.Count; stateIndex++)
			{
				var state = group.States[stateIndex];

				for (var triggerIndex = 0; triggerIndex < state.StateTriggers.Count; triggerIndex++)
				{
					var trigger = state.StateTriggers[triggerIndex];

					if (trigger is AdaptiveTrigger adaptiveTrigger && adaptiveTrigger.IsTriggerActive)
					{
						adaptiveTrigger.IsTriggerActive = false;
					}
				}
			}
		}
	}
}