using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui
{
	public abstract class Application : IApplication
	{
		static object? GlobalLock;
		static Application? AppInstance;

		IServiceProvider? _serviceProvider;
		IMauiContext? _context;

		protected Application()
		{
			GlobalLock = new object();

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

		public IServiceProvider? Services => _serviceProvider;

		public IMauiContext? Context => _context;

		public event EventHandler? Created;

		public event EventHandler? Resumed;

		public event EventHandler? Paused;

		public event EventHandler? Stopped;

		public abstract IWindow CreateWindow(IActivationState state);

		public virtual void OnCreated()
		{
			Created?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnResumed()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnPaused()
		{
			Paused?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnStopped()
		{
			Stopped?.Invoke(this, EventArgs.Empty);
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