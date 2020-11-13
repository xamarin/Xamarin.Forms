using ERect = ElmSharp.Rect;

namespace Microsoft.Maui
{
	public interface ITizenViewHandler : IViewHandler
	{
		void SetContext(CoreUIAppContext context);

		CoreUIAppContext? Context { get; }

		void SetParent(ITizenViewHandler parent);

		ITizenViewHandler? Parent { get; }

		ERect GetNativeContentGeometry();
	}
}