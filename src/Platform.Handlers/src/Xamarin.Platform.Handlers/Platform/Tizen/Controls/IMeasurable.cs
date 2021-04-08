using ElmSharp;

namespace Xamarin.Platform.Tizen
{
	public interface IMeasurable
	{
		Size Measure(int availableWidth, int availableHeight);
	}
}
