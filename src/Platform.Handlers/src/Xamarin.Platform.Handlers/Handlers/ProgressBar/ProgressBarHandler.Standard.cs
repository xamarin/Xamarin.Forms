using System;

namespace Xamarin.Platform.Handlers
{
	public partial class ProgressBarHandler : AbstractViewHandler<IProgress, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();
	}
}