namespace Xamarin.Forms
{
	public class StateTriggerBase : BindableObject
	{
		protected StateTriggerBase()
		{

		}

		internal StateTriggerPrecedence CurrentPrecedence { get; set; } = 0;

		//internal VisualState VisualState => this.GetParent() as VisualState;

		protected void SetActive(bool active)
		{
			SetActivePrecedence(active ? StateTriggerPrecedence.CustomTrigger : StateTriggerPrecedence.Inactive);
		}

		internal void SetActivePrecedence(StateTriggerPrecedence precedence)
		{
			if (CurrentPrecedence == precedence)
				return;

			CurrentPrecedence = precedence;

			//VisualState?.VisualStateGroup?.RefreshStateTriggers(null);
		}
	}
}