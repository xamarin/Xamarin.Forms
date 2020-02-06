using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Forms.DualScreen
{
	public class DualScreenStateTrigger : StateTriggerBase
	{
		readonly IDualScreenService _dualScreenService;

		public DualScreenStateTrigger()
		{
			UpdateState();

			_dualScreenService = DependencyService.Get<IDualScreenService>();

			if (_dualScreenService != null && !DesignMode.IsDesignModeEnabled)
			{
				var weakEvent = new WeakEventListener<DualScreenStateTrigger, object, EventArgs>(this)
				{
					OnEventAction = (instance, source, eventArgs) => OnScreenChanged(source, eventArgs),
					OnDetachAction = (listener) => { _dualScreenService.OnScreenChanged -= listener.OnEvent; }
				};

				_dualScreenService.OnScreenChanged += weakEvent.OnEvent;
			}
		}

		public bool IsSpanned
		{
			get => (bool)GetValue(IsSpannedProperty);
			set => SetValue(IsSpannedProperty, value);
		}

		public static readonly BindableProperty IsSpannedProperty =
			BindableProperty.Create(nameof(IsSpanned), typeof(bool), typeof(DualScreenStateTrigger), false,
				propertyChanged: OnIsSpannedChanged);

		static void OnIsSpannedChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((DualScreenStateTrigger)bindable).UpdateState();
		}

		void OnScreenChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		void UpdateState()
		{
			bool isActive = false;

			if (_dualScreenService == null)
			{
				SetActive(isActive);
				return;
			}

			isActive = IsSpanned.Equals(_dualScreenService.IsSpanned);

			SetActive(isActive);
		}
	}
}