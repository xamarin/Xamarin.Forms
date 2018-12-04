using System.ComponentModel;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IFrameController
	{
		//note to implementor: implement these methods explicitly
		void SetContentPadding(Thickness value);
	}
}