using System;

namespace Xamarin.Platform.Handlers
{
	public partial class ActivityIndicatorHandler : AbstractViewHandler<IActivityIndicator, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();
	}
}