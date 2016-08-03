namespace Xamarin.Forms
{
	public interface IViewController : IVisualElementController
	{
		EffectiveFlowDirection EffectiveFlowDirection { get; }
	}
}