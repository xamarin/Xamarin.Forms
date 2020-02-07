using System.ComponentModel;

namespace Xamarin.Forms.DualScreen
{
	public class DualScreenSpanModeStateTrigger : StateTriggerBase
	{
		public DualScreenSpanModeStateTrigger()
		{
			UpdateState();

			if (!DesignMode.IsDesignModeEnabled)
			{
				var weakEvent = new WeakEventListener<DualScreenSpanModeStateTrigger, object, PropertyChangedEventArgs>(this)
				{
					OnEventAction = (instance, source, eventArgs) => OnDualScreenInfoPropertyChanged(source, eventArgs),
					OnDetachAction = (listener) => { DualScreenInfo.Current.PropertyChanged -= listener.OnEvent; }
				};

				DualScreenInfo.Current.PropertyChanged += weakEvent.OnEvent;
			}
		}

		public TwoPaneViewMode SpanMode
		{
			get => (TwoPaneViewMode)GetValue(SpanModeProperty);
			set => SetValue(SpanModeProperty, value);
		}

		public static readonly BindableProperty SpanModeProperty =
			BindableProperty.Create(nameof(SpanMode), typeof(TwoPaneViewMode), typeof(DualScreenSpanModeStateTrigger), default(TwoPaneViewMode),
				propertyChanged: OnSpanModeChanged);

		static void OnSpanModeChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((DualScreenSpanModeStateTrigger)bindable).UpdateState();
		}

		void OnDualScreenInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateState();
		}

		void UpdateState()
		{
			var spanMode = DualScreenInfo.Current.SpanMode;

			switch (SpanMode)
			{
				case TwoPaneViewMode.SinglePane:
					SetActive(spanMode == TwoPaneViewMode.SinglePane);
					break;
				case TwoPaneViewMode.Tall:
					SetActive(spanMode == TwoPaneViewMode.Tall);
					break;
				case TwoPaneViewMode.Wide:
					SetActive(spanMode == TwoPaneViewMode.Wide);
					break;
			}
		}
	}
}