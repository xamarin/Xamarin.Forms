using System;
using Xamarin.Forms.Platform.MacOS;

namespace Xamarin.Forms
{
	public static class Forms
	{
		public static bool IsInitialized { get; private set; }

		public static void Init()
		{
			if (IsInitialized)
				return;
			IsInitialized = true;
			//	Color.Accent = Color.FromRgba(50, 79, 133, 255);

			//Log.Listeners.Add(new DelegateLogListener((c, m) => Trace.WriteLine(m, c)));

			Device.OS = TargetPlatform.iOS;
			Device.PlatformServices = new MacOSPlatformServices();
			Device.Info = new MacOSDeviceInfo();

			Registrar.RegisterAll(new[] { typeof(ExportRendererAttribute), typeof(ExportCellAttribute), typeof(ExportImageSourceHandlerAttribute) });

			Device.Idiom = TargetIdiom.Desktop;

			//ExpressionSearch.Default = new iOSExpressionSearch();

		}
	}
}

