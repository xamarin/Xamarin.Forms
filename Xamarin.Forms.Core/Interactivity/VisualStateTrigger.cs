using System;

namespace Xamarin.Forms
{
	public class VisualStateTrigger : TriggerAction<VisualElement>
	{
		public string State { get; set; }

		public VisualElement Target { get; set; }

		protected override void Invoke(VisualElement sender)
		{
			VisualElement visualElement = Target ?? sender;
			if (visualElement != null && !string.IsNullOrEmpty(State))
			{
				VisualStateManager.GoToState(visualElement, State);
			}
		}
	}
}