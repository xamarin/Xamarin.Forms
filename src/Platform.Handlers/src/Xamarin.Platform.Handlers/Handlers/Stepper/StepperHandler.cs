namespace Xamarin.Platform.Handlers
{
	public partial class StepperHandler
	{
		public static PropertyMapper<IStepper, StepperHandler> StepperMapper = new PropertyMapper<IStepper, StepperHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IStepper.Minimum)] = MapMinimum,
			[nameof(IStepper.Maximum)] = MapMaximum,
			[nameof(IStepper.Increment)] = MapIncrement,
			[nameof(IStepper.Value)] = MapValue
#if __ANDROID__ || NETCOREAPP
			,[nameof(IStepper.IsEnabled)] = MapIsEnabled
#endif
		};

		public static void MapMinimum(StepperHandler handler, IStepper stepper)
		{
			ViewHandler.CheckParameters(handler, stepper);
			handler.TypedNativeView?.UpdateMinimum(stepper);
		}

		public static void MapMaximum(StepperHandler handler, IStepper stepper)
		{
			ViewHandler.CheckParameters(handler, stepper);
			handler.TypedNativeView?.UpdateMaximum(stepper);
		}

		public static void MapIncrement(StepperHandler handler, IStepper stepper)
		{
			ViewHandler.CheckParameters(handler, stepper);
			handler.TypedNativeView?.UpdateIncrement(stepper);
		}

		public static void MapValue(StepperHandler handler, IStepper stepper)
		{
			ViewHandler.CheckParameters(handler, stepper);
			handler.TypedNativeView?.UpdateValue(stepper);
		}

#if __ANDROID__ || NETCOREAPP
		public static void MapIsEnabled(StepperHandler handler, IStepper stepper)
		{
			ViewHandler.CheckParameters(handler, stepper);
			handler.TypedNativeView?.UpdateIsEnabled(stepper);
		}
#endif
		public StepperHandler() : base(StepperMapper)
		{

		}

		public StepperHandler(PropertyMapper mapper) : base(mapper ?? StepperMapper)
		{

		}
	}
}