using System;

namespace Xamarin.Platform.Handlers
{
	public partial class BoxViewHandler : AbstractViewHandler<IBox, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();
	}
}