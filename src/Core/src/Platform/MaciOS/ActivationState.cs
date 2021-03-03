namespace Microsoft.Maui
{
	public class ActivationState : IActivationState
	{
		public IMauiContext Context { get; }

		public ActivationState(IMauiContext context)
		{
			Context = context;
		}
	}
}