using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IProgress : IView
	{
		double Progress { get; }
		Color ProgressColor { get; }
	}
}