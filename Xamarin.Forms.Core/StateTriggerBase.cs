using System.ComponentModel;

namespace Xamarin.Forms
{
	public class StateTriggerBase : BindableObject
	{
		internal bool IsTriggerActive { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal VisualState VisualState { get; set; }

		protected void SetActive(bool active)
		{
			IsTriggerActive = active;

			VisualState?.VisualStateGroup?.UpdateStateTriggers();
		}
	}
}