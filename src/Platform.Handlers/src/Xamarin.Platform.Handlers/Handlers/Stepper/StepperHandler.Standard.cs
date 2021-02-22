using System;

namespace Xamarin.Platform.Handlers
{
	public partial class StepperHandler : AbstractViewHandler<IStepper, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();
	}
}