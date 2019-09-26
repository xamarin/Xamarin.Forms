using System;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	public class VisualStateTrigger : TriggerAction<VisualElement>
	{
		public string GoToState { get; set; }

		public VisualElement Target { get; set; }

		protected override void Invoke(VisualElement sender)
		{
			VisualElement visualElement = Target ?? sender;
			if (visualElement != null && !string.IsNullOrEmpty(GoToState))
			{
				VisualStateManager.GoToState(visualElement, GoToState);
			}
		}
	}
}