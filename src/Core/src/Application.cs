using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui
{
	public abstract class Application : IApplication
	{
		static object? GlobalLock;
		static Application? AppInstance;

		IWindow? _mainWindow;

		readonly WindowCollection _windows;
		IServiceProvider? _serviceProvider;
		IMauiContext? _context;

		protected Application()
		{
			GlobalLock = new object();
			_windows = new WindowCollection();

			if (AppInstance != null)
				throw new InvalidOperationException($"Only one {nameof(Application)} instance is allowed");

			lock (GlobalLock)
			{
				AppInstance = this;
			}
		}

		static public Application? Current
		{
			get
			{
				return AppInstance;
			}
		}

		public IWindow? MainWindow
		{
			get
			{
				return _mainWindow;
			}
			set
			{
				if (value != _mainWindow)
				{
					_mainWindow = value;
				}
			}
		}

		public WindowCollection Windows => _windows;

		public IServiceProvider? Services => _serviceProvider;

		public IMauiContext? Context => _context;

		public event EventHandler? Created;

		public event EventHandler? Resumed;

		public event EventHandler? Paused;

		public event EventHandler? Stopped;

		public void Run()
		{
			Run(null);
		}

		public void Run(IWindow? window)
		{
			if (window != null)
			{
				if (Windows.HasItem(window) == false)
				{
					Windows.Add(window);
				}

				if (MainWindow == null)
				{
					MainWindow = window;
				}

				Window? win = window as Window;
				win?.Show();
			}
		}

		// Move to abstract
		public virtual IAppHostBuilder CreateBuilder() => CreateDefaultBuilder();

		public virtual void OnCreated()
		{
			Created?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnResumed()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
			MainWindow?.OnResumed();
		}

		public virtual void OnPaused()
		{
			Paused?.Invoke(this, EventArgs.Empty);
			MainWindow?.OnPaused();
		}

		public virtual void OnStopped()
		{
			Stopped?.Invoke(this, EventArgs.Empty);
		}

		public static IAppHostBuilder CreateDefaultBuilder()
		{
			var builder = new AppHostBuilder();

			builder.UseMauiHandlers();
			builder.

			return builder;
		}

		internal void SetServiceProvider(IServiceProvider provider)
		{
			_serviceProvider = provider;
			SetHandlerContext(provider.GetService<IMauiContext>());
		}

		internal void SetHandlerContext(IMauiContext? context)
		{
			_context = context;
		}
	}
}