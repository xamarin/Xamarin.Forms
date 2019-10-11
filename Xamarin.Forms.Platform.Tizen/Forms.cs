using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Tizen;
using ElmSharp;
using Tizen.Applications;
using TSystemInfo = Tizen.System.Information;
using ELayout = ElmSharp.Layout;
using DeviceOrientation = Xamarin.Forms.Internals.DeviceOrientation;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public struct InitializationOptions
	{
		public struct EffectScope
		{
			public string Name;
			public ExportEffectAttribute[] Effects;
		}

		public InitializationOptions(CoreApplication application, bool useDeviceIndependentPixel, HandlerAttribute[] handlers)
		{
			this = default(InitializationOptions);
			Context = application;
			UseDeviceIndependentPixel = useDeviceIndependentPixel;
			Handlers = handlers;
			Assemblies = null;
		}

		public InitializationOptions(CoreApplication application, bool useDeviceIndependentPixel, params Assembly[] assemblies)
		{
			this = default(InitializationOptions);
			Context = application;
			UseDeviceIndependentPixel = useDeviceIndependentPixel;
			Handlers = null;
			Assemblies = assemblies;
		}

		public CoreApplication Context;
		public bool UseDeviceIndependentPixel;
		public HandlerAttribute[] Handlers;
		public Assembly[] Assemblies;
		public EffectScope[] EffectScopes;
		public InitializationFlags Flags;
	}

	public static class Forms
	{
		static Lazy<string> s_profile = new Lazy<string>(() =>
		{
			//TODO : Fix me if elm_config_profile_get() unavailable
			return Elementary.GetProfile();
		});

		static Lazy<int> s_dpi = new Lazy<int>(() =>
		{
			int dpi = 0;
			if (s_profile.Value == "tv")
			{
				// Use fixed DPI value (72) if TV profile
				return 72;
			}
			TSystemInfo.TryGetValue<int>("http://tizen.org/feature/screen.dpi", out dpi);
			return dpi;
		});

		static Lazy<double> s_elmScale = new Lazy<double>(() =>
		{
			// 72.0 is from EFL which is using fixed DPI value (72.0) to determine font size internally. Thus, we are restoring the size by deviding the DPI by 72.0 here.
			return s_dpi.Value / 72.0 / Elementary.GetScale();
		});

		static Lazy<DeviceOrientation> s_naturalOrientation = new Lazy<DeviceOrientation>(() =>
		{
			int width = 0;
			int height = 0;
			TSystemInfo.TryGetValue<int>("http://tizen.org/feature/screen.width", out width);
			TSystemInfo.TryGetValue<int>("http://tizen.org/feature/screen.height", out height);

			if (height >= width)
			{
				return DeviceOrientation.Portrait;
			}
			else
			{
				return DeviceOrientation.Landscape;
			}
		});

		class TizenDeviceInfo : DeviceInfo
		{
			readonly Size pixelScreenSize;

			readonly Size scaledScreenSize;

			readonly double scalingFactor;

			readonly string profile;

			public override Size PixelScreenSize
			{
				get
				{
					return this.pixelScreenSize;
				}
			}

			public override Size ScaledScreenSize
			{
				get
				{
					return this.scaledScreenSize;
				}
			}

			public override double ScalingFactor
			{
				get
				{
					return this.scalingFactor;
				}
			}

			public string Profile
			{
				get
				{
					return this.profile;
				}
			}

			public TizenDeviceInfo()
			{
				int width = 0;
				int height = 0;

				TSystemInfo.TryGetValue("http://tizen.org/feature/screen.width", out width);
				TSystemInfo.TryGetValue("http://tizen.org/feature/screen.height", out height);

				scalingFactor = 1.0;  // scaling is disabled, we're using pixels as Xamarin's geometry units
				if (_useDeviceIndependentPixel)
				{
					scalingFactor = s_dpi.Value / 160.0;
				}

				pixelScreenSize = new Size(width, height);
				scaledScreenSize = new Size(width / scalingFactor, height / scalingFactor);
				profile = s_profile.Value;
			}
		}

		static bool _useDeviceIndependentPixel = false;

		public static event EventHandler<ViewInitializedEventArgs> ViewInitialized;

		public static CoreApplication Context
		{
			get;
			internal set;
		}

		public static EvasObject NativeParent
		{
			get; internal set;
		}

		public static ELayout BaseLayout => NativeParent as ELayout;

		public static bool IsInitialized
		{
			get;
			private set;
		}

		public static DeviceOrientation NaturalOrientation => s_naturalOrientation.Value;

		internal static TizenTitleBarVisibility TitleBarVisibility
		{
			get;
			private set;
		}

		internal static void SendViewInitialized(this VisualElement self, EvasObject nativeView)
		{
			EventHandler<ViewInitializedEventArgs> viewInitialized = Forms.ViewInitialized;
			if (viewInitialized != null)
			{
				viewInitialized.Invoke(self, new ViewInitializedEventArgs
				{
					View = self,
					NativeView = nativeView
				});
			}
		}

		static IReadOnlyList<string> s_flags;
		public static IReadOnlyList<string> Flags => s_flags ?? (s_flags = new List<string>().AsReadOnly());

		public static void SetFlags(params string[] flags)
		{
			if (IsInitialized)
			{
				throw new InvalidOperationException($"{nameof(SetFlags)} must be called before {nameof(Init)}");
			}

			s_flags = flags.ToList().AsReadOnly();
		}

		public static void SetTitleBarVisibility(TizenTitleBarVisibility visibility)
		{
			TitleBarVisibility = visibility;
		}

		public static void Init(CoreApplication application)
		{
			Init(application, false);
		}

		public static void Init(CoreApplication application, bool useDeviceIndependentPixel)
		{
			_useDeviceIndependentPixel = useDeviceIndependentPixel;
			SetupInit(application, null);
		}

		public static void Init(InitializationOptions options)
		{
			SetupInit(options.Context, options);
		}

		static void SetupInit(CoreApplication application, InitializationOptions? maybeOptions = null)
		{
			Context = application;

			if (!IsInitialized)
			{
				Internals.Log.Listeners.Add(new XamarinLogListener());
				if (System.Threading.SynchronizationContext.Current == null)
				{
					TizenSynchronizationContext.Initialize();
				}
				Elementary.Initialize();
				Elementary.ThemeOverlay();
			}

			Device.PlatformServices = new TizenPlatformServices();
			if (Device.info != null)
			{
				((TizenDeviceInfo)Device.info).Dispose();
				Device.info = null;
			}

			Device.Info = new Forms.TizenDeviceInfo();
			Device.SetFlags(s_flags);

			if (!Forms.IsInitialized)
			{
				if (maybeOptions.HasValue)
				{
					var options = maybeOptions.Value;
					_useDeviceIndependentPixel = options.UseDeviceIndependentPixel;

					if (options.Assemblies != null)
					{
						TizenPlatformServices.AppDomain.CurrentDomain.AddAssemblies(options.Assemblies);
					}

					// renderers
					if (options.Handlers != null)
					{
						Registrar.RegisterRenderers(options.Handlers);
					}
					else
					{
						Registrar.RegisterAll(new Type[]
						{
							typeof(ExportRendererAttribute),
							typeof(ExportImageSourceHandlerAttribute),
							typeof(ExportCellAttribute),
							typeof(ExportHandlerAttribute)
						});
					}

					// effects
					var effectScopes = options.EffectScopes;
					if (effectScopes != null)
					{
						for (var i = 0; i < effectScopes.Length; i++)
						{
							var effectScope = effectScopes[0];
							Registrar.RegisterEffects(effectScope.Name, effectScope.Effects);
						}
					}

					// css
					var flags = options.Flags;
					var noCss = (flags & InitializationFlags.DisableCss) != 0;
					if (!noCss)
						Registrar.RegisterStylesheets();
				}
				else
				{
					// In .NETCore, AppDomain feature is not supported.
					// The list of assemblies returned by AppDomain.GetAssemblies() method should be registered manually.
					// The assembly of the executing application and referenced assemblies of it are added into the list here.
					TizenPlatformServices.AppDomain.CurrentDomain.RegisterAssemblyRecursively(application.GetType().GetTypeInfo().Assembly);
					Registrar.RegisterAll(new Type[]
					{
						typeof(ExportRendererAttribute),
						typeof(ExportImageSourceHandlerAttribute),
						typeof(ExportCellAttribute),
						typeof(ExportHandlerAttribute)
					});
				}
			}

			string profile = ((TizenDeviceInfo)Device.Info).Profile;
			if (profile == "mobile")
			{
				Device.SetIdiom(TargetIdiom.Phone);
			}
			else if (profile == "tv")
			{
				Device.SetIdiom(TargetIdiom.TV);
			}
			else if (profile == "desktop")
			{
				Device.SetIdiom(TargetIdiom.Desktop);
			}
			else if (profile == "wearable")
			{
				Device.SetIdiom(TargetIdiom.Watch);
			}
			else
			{
				Device.SetIdiom(TargetIdiom.Unsupported);
			}
			Color.SetAccent(GetAccentColor(profile));
			ExpressionSearch.Default = new TizenExpressionSearch();
			IsInitialized = true;
		}

		static Color GetAccentColor(string profile)
		{
			// On Windows Phone, this is the complementary color chosen by the user.
			// Good Windows Phone applications use this as part of their styling to provide a native look and feel.
			// On iOS and Android this instance is set to a contrasting color that is visible on the default
			// background but is not the same as the default text color.

			switch (profile)
			{
				case "mobile":
					// [Tizen_3.0]Basic_Interaction_GUI_[REF].xlsx Theme 001 (Default) 1st HSB: 188 70 80
					return Color.FromRgba(61, 185, 204, 255);
				case "tv":
					return Color.FromRgba(15, 15, 15, 230);
				case "wearable":
					// Theme A (Default) 1st HSB: 207 75 16
					return Color.FromRgba(10, 27, 41, 255);
				default:
					return Color.Black;
			}
		}

		/// <summary>
		/// Converts the dp into pixel considering current DPI value.
		/// </summary>
		/// <remarks>
		/// Use this API if you want to get pixel size without scaling factor.
		/// </remarks>
		/// <param name="dp"></param>
		/// <returns></returns>
		public static int ConvertToPixel(double dp)
		{
			return (int)Math.Round(dp * s_dpi.Value / 160.0);
		}

		/// <summary>
		/// Converts the dp into pixel by using scaling factor.
		/// </summary>
		/// <remarks>
		/// Use this API if you want to get pixel size from dp by using scaling factor.
		/// If the scaling factor is 1.0 by user setting, the same value is returned. This is mean that user doesn't want to pixel independent metric.
		/// </remarks>
		/// <param name="dp"></param>
		/// <returns></returns>
		public static int ConvertToScaledPixel(double dp)
		{
			return (int)Math.Round(dp * Device.Info.ScalingFactor);
		}

		/// <summary>
		/// Converts the pixel into dp value by using scaling factor.
		/// </summary>
		/// <remarks>
		/// If the scaling factor is 1.0 by user setting, the same value is returned. This is mean that user doesn't want to pixel independent metric.
		/// </remarks>
		/// <param name="pixel"></param>
		/// <returns></returns>
		public static double ConvertToScaledDP(int pixel)
		{
			return pixel / Device.Info.ScalingFactor;
		}

		/// <summary>
		/// Converts the pixel into dp value by using scaling factor.
		/// </summary>
		/// <remarks>
		/// If the scaling factor is 1.0 by user setting, the same value is returned. This is mean that user doesn't want to pixel independent metric.
		/// </remarks>
		/// <param name="pixel"></param>
		/// <returns></returns>
		public static double ConvertToScaledDP(double pixel)
		{
			return pixel / Device.Info.ScalingFactor;
		}

		/// <summary>
		/// Converts the sp into EFL's font size metric (EFL point).
		/// </summary>
		/// <param name="sp"></param>
		/// <returns></returns>
		public static int ConvertToEflFontPoint(double sp)
		{
			return (int)Math.Round(sp * s_elmScale.Value);
		}

		/// <summary>
		/// Convert the EFL's point into sp.
		/// </summary>
		/// <param name="eflPt"></param>
		/// <returns></returns>
		public static double ConvertToDPFont(int eflPt)
		{
			return eflPt / s_elmScale.Value;
		}

		/// <summary>
		/// Get the EFL's profile
		/// </summary>
		/// <returns></returns>
		public static string GetProfile()
		{
			return s_profile.Value;
		}


		// for internal use only
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Preload()
		{
			Elementary.Initialize();
			Elementary.ThemeOverlay();
			var window = new PreloadedWindow();
			var platform = new PreloadedPlatform(window.BaseLayout);
			TSystemInfo.TryGetValue("http://tizen.org/feature/screen.width", out int width);
			TSystemInfo.TryGetValue("http://tizen.org/feature/screen.height", out int height);
		}
	}

	class TizenExpressionSearch : ExpressionVisitor, IExpressionSearch
	{
		List<object> _results;
		Type _targetType;

		public List<T> FindObjects<T>(Expression expression) where T : class
		{
			_results = new List<object>();
			_targetType = typeof(T);
			Visit(expression);
			return _results.Select(o => o as T).ToList();
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression is ConstantExpression && node.Member is FieldInfo)
			{
				var container = ((ConstantExpression)node.Expression).Value;
				var value = ((FieldInfo)node.Member).GetValue(container);

				if (_targetType.IsInstanceOfType(value))
					_results.Add(value);
			}
			return base.VisitMember(node);
		}
	}
}
