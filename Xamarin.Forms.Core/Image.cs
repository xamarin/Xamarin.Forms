using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_ImageRenderer))]
	public class Image : View, IImageController, IElementConfiguration<Image>, IViewController, IImageElement
	{
		public static readonly BindableProperty SourceProperty = ImageElement.SourceProperty;

		public static readonly BindableProperty AspectProperty = ImageElement.AspectProperty;

		public static readonly BindableProperty IsOpaqueProperty = ImageElement.IsOpaqueProperty;

		internal static readonly BindablePropertyKey IsLoadingPropertyKey = BindableProperty.CreateReadOnly(nameof(IsLoading), typeof(bool), typeof(Image), default(bool));

		public static readonly BindableProperty IsLoadingProperty = IsLoadingPropertyKey.BindableProperty;

		public enum AnimationPlayBehaviorValue
		{
			None,
			OnLoad,
			OnStart
		};

		public static readonly BindableProperty AnimationPlayBehaviorProperty = BindableProperty.Create(nameof(AnimationPlayBehavior), typeof(AnimationPlayBehaviorValue), typeof(Image), AnimationPlayBehaviorValue.None);

		public static readonly BindableProperty IsAnimationPlayingProperty = BindableProperty.Create (nameof(IsAnimationPlaying), typeof (bool), typeof (Image), false);

		readonly Lazy<PlatformConfigurationRegistry<Image>> _platformConfigurationRegistry;

		public Image()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Image>>(() => new PlatformConfigurationRegistry<Image>(this));
		}

		public Aspect Aspect
		{
			get { return (Aspect)GetValue(AspectProperty); }
			set { SetValue(AspectProperty, value); }
		}

		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
		}

		public bool IsOpaque
		{
			get { return (bool)GetValue(IsOpaqueProperty); }
			set { SetValue(IsOpaqueProperty, value); }
		}

		public bool IsAnimationPlaying
		{
			get { return (bool)GetValue(IsAnimationPlayingProperty); }
		}

		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public AnimationPlayBehaviorValue AnimationPlayBehavior
		{
			get { return (AnimationPlayBehaviorValue)GetValue(AnimationPlayBehaviorProperty); }
			set { SetValue(AnimationPlayBehaviorProperty, value); }
		}

		public void StartAnimation()
		{
			if (AnimationPlayBehavior != AnimationPlayBehaviorValue.None)
				SetValue(IsAnimationPlayingProperty, true);
		}

		public void StopAnimation()
		{
			if (AnimationPlayBehavior != AnimationPlayBehaviorValue.None)
				SetValue (IsAnimationPlayingProperty, false);
		}

		protected override void OnBindingContextChanged()
		{
			ImageElement.OnBindingContextChanged(this, this);
			base.OnBindingContextChanged();
		}

		[Obsolete("OnSizeRequest is obsolete as of version 2.2.0. Please use OnMeasure instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			SizeRequest desiredSize = base.OnSizeRequest(double.PositiveInfinity, double.PositiveInfinity);
			return ImageElement.Measure(this, desiredSize, widthConstraint, heightConstraint);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetIsLoading(bool isLoading)
		{
			SetValue(IsLoadingPropertyKey, isLoading);
		}

		public IPlatformElementConfiguration<T, Image> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void IImageElement.OnImageSourcesSourceChanged(object sender, EventArgs e) =>
			ImageElement.ImageSourcesSourceChanged(this, e);

		void IImageElement.RaiseImageSourcePropertyChanged() => OnPropertyChanged(nameof(Source));
	}
}