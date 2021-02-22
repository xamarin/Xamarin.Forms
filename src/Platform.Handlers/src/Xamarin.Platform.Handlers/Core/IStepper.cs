namespace Xamarin.Platform
{
	public interface IStepper : IRange
	{
		double Increment { get; }
	}
}