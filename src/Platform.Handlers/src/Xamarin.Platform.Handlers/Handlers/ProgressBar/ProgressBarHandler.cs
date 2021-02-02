namespace Xamarin.Platform.Handlers
{
	public partial class ProgressBarHandler
	{
		public static PropertyMapper<IProgress, ProgressBarHandler> ProgressMapper = new PropertyMapper<IProgress, ProgressBarHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IProgress.Progress)] = MapProgress,
			[nameof(IProgress.ProgressColor)] = MapProgressColor,
#if __IOS__
			[nameof(IView.BackgroundColor)] = MapBackgroundColor
#endif
		};

		public static void MapProgress(ProgressBarHandler handler, IProgress progress)
		{
			ViewHandler.CheckParameters(handler, progress);
			handler.TypedNativeView?.UpdateProgress(progress);
		}

		public static void MapProgressColor(ProgressBarHandler handler, IProgress progress)
		{
			ViewHandler.CheckParameters(handler, progress);
			handler.TypedNativeView?.UpdateProgressColor(progress);
		}

		public static void MapBackgroundColor(ProgressBarHandler handler, IProgress progress)
		{
			ViewHandler.CheckParameters(handler, progress);
			handler.TypedNativeView?.UpdateBackgroundColor(progress);
		}

		public ProgressBarHandler() : base(ProgressMapper)
		{

		}

		public ProgressBarHandler(PropertyMapper mapper) : base(mapper ?? ProgressMapper)
		{

		}
	}
}
