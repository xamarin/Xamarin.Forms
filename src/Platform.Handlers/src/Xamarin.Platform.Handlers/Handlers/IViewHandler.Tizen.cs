using ElmSharp;

namespace Xamarin.Platform
{
	public interface ITizenViewHandler : IViewHandler
	{
		void SetContext(CoreUIAppContext context);

		CoreUIAppContext? Context { get; }

		void SetParent(ITizenViewHandler parent);

		ITizenViewHandler? Parent { get; }

		Rect GetNativeContentGeometry();
	}
}