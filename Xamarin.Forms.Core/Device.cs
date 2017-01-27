using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public static class Device
	{
		public const string iOS = "iOS";
		public const string Android = "Android";
		public const string WinPhone = "WinPhone";
		public const string Windows = "Windows";
		public const string macOS = "macOS";

		internal static DeviceInfo info;

		static IPlatformServices s_platformServices;

		public static TargetIdiom Idiom { get; internal set; }

		[Obsolete("Use RuntimePlatform instead.")]
#pragma warning disable 0618
		public static TargetPlatform OS
		{
			get
			{
				TargetPlatform platform;
				if (Enum.TryParse(RuntimePlatform, out platform))
					return platform;
				return TargetPlatform.Other;
			}
		}
#pragma warning restore 0618

		public static string RuntimePlatform => PlatformServices.RuntimePlatform;

		internal static DeviceInfo Info
		{
			get
			{
				if (info == null)
					throw new InvalidOperationException("You MUST call Xamarin.Forms.Init(); prior to using it.");
				return info;
			}
			set { info = value; }
		}

		internal static bool IsInvokeRequired
		{
			get { return PlatformServices.IsInvokeRequired; }
		}

		internal static IPlatformServices PlatformServices
		{
			get
			{
				if (s_platformServices == null)
					throw new InvalidOperationException("You MUST call Xamarin.Forms.Init(); prior to using it.");
				return s_platformServices;
			}
			set { s_platformServices = value; }
		}

		public static void BeginInvokeOnMainThread(Action action)
		{
			PlatformServices.BeginInvokeOnMainThread(action);
		}

		public static double GetNamedSize(NamedSize size, Element targetElement)
		{
			return GetNamedSize(size, targetElement.GetType());
		}

		public static double GetNamedSize(NamedSize size, Type targetElementType)
		{
			return GetNamedSize(size, targetElementType, false);
		}

		[Obsolete("Use switch(RuntimePlatform) instead.")]
		public static void OnPlatform(Action iOS = null, Action Android = null, Action WinPhone = null, Action Default = null)
		{
			switch (OS)
			{
				case TargetPlatform.iOS:
					if (iOS != null)
						iOS();
					else if (Default != null)
						Default();
					break;
				case TargetPlatform.Android:
					if (Android != null)
						Android();
					else if (Default != null)
						Default();
					break;
				case TargetPlatform.Windows:
				case TargetPlatform.WinPhone:
					if (WinPhone != null)
						WinPhone();
					else if (Default != null)
						Default();
					break;
				case TargetPlatform.Other:
					if (Default != null)
						Default();
					break;
			}
		}

		[Obsolete("Use switch(RuntimePlatform) instead.")]
		public static T OnPlatform<T>(T iOS, T Android, T WinPhone)
		{
			switch (OS)
			{
				case TargetPlatform.iOS:
					return iOS;
				case TargetPlatform.Android:
					return Android;
				case TargetPlatform.Windows:
				case TargetPlatform.WinPhone:
					return WinPhone;
			}

			return iOS;
		}

		public static void OpenUri(Uri uri)
		{
			PlatformServices.OpenUriAction(uri);
		}

		public static void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			PlatformServices.StartTimer(interval, callback);
		}

		internal static Assembly[] GetAssemblies()
		{
			return PlatformServices.GetAssemblies();
		}

		internal static double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
		{
			return PlatformServices.GetNamedSize(size, targetElementType, useOldSizes);
		}

		internal static Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
		{
			return PlatformServices.GetStreamAsync(uri, cancellationToken);
		}

		public static class Styles
		{
			public static readonly string TitleStyleKey = "TitleStyle";

			public static readonly string SubtitleStyleKey = "SubtitleStyle";

			public static readonly string BodyStyleKey = "BodyStyle";

			public static readonly string ListItemTextStyleKey = "ListItemTextStyle";

			public static readonly string ListItemDetailTextStyleKey = "ListItemDetailTextStyle";

			public static readonly string CaptionStyleKey = "CaptionStyle";

			public static readonly Style TitleStyle = new Style(typeof(Label)) { BaseResourceKey = TitleStyleKey };

			public static readonly Style SubtitleStyle = new Style(typeof(Label)) { BaseResourceKey = SubtitleStyleKey };

			public static readonly Style BodyStyle = new Style(typeof(Label)) { BaseResourceKey = BodyStyleKey };

			public static readonly Style ListItemTextStyle = new Style(typeof(Label)) { BaseResourceKey = ListItemTextStyleKey };

			public static readonly Style ListItemDetailTextStyle = new Style(typeof(Label)) { BaseResourceKey = ListItemDetailTextStyleKey };

			public static readonly Style CaptionStyle = new Style(typeof(Label)) { BaseResourceKey = CaptionStyleKey };
		}
	}
}