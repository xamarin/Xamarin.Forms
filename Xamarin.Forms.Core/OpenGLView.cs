﻿using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_OpenGLViewRenderer))]
	public sealed class OpenGLView : View, IOpenGlViewController, IElementConfiguration<OpenGLView>
	{
		#region Statics

		public static readonly BindableProperty HasRenderLoopProperty = BindableProperty.Create("HasRenderLoop", typeof(bool), typeof(OpenGLView), default(bool));

		readonly Lazy<PlatformConfigurationRegistry<OpenGLView>> _platformConfigurationRegistry;

		#endregion

		public bool HasRenderLoop
		{
			get { return (bool)GetValue(HasRenderLoopProperty); }
			set { SetValue(HasRenderLoopProperty, value); }
		}

		public Action<Rectangle> OnDisplay { get; set; }

		event EventHandler IOpenGlViewController.DisplayRequested
		{
			add { DisplayRequested += value; }
			remove { DisplayRequested -= value; }
		}

		public void Display()
		{
			DisplayRequested?.Invoke(this, EventArgs.Empty);
		}

		event EventHandler DisplayRequested;

		public OpenGLView()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<OpenGLView>>(() => new PlatformConfigurationRegistry<OpenGLView>(this));
		}

		public IPlatformElementConfiguration<T, OpenGLView> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}