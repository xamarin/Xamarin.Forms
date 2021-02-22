namespace Xamarin.Platform
{
	public interface IRange : IView
	{
		double Minimum { get; }
		double Maximum { get; }
		double Value { get; set; }
	}
}