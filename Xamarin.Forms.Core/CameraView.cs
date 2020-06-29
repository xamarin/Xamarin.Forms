using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CameraViewRenderer))]
	public class CameraView: View, IElementConfiguration<CameraView>
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void VerifyCameraViewFlagEnabled(
			string constructorHint = null,
			[CallerMemberName] string memberName = "")
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(CameraView), ExperimentalFlags.CameraViewExperimental, memberName: memberName);
		}

		public event EventHandler<MediaCapturedEventArgs> MediaCaptured;

		public event EventHandler<string> MediaCaptureFailed;

		public event EventHandler<bool> OnAvailable;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler ShutterClicked;

		public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(CameraView), false);

		public bool IsBusy
		{
			get => (bool)GetValue(IsBusyProperty);
			set => SetValue(IsBusyProperty, value);
		}

		public static readonly BindableProperty IsAvailableProperty = BindableProperty.Create(nameof(IsAvailable), typeof(bool), typeof(CameraView), false, 
			propertyChanged: (b, o, n) => ((CameraView)b).OnAvailable?.Invoke(b, (bool)n));

		public bool IsAvailable
		{
			get => (bool)GetValue(IsAvailableProperty);
			set => SetValue(IsAvailableProperty, value);
		}

		public static readonly BindableProperty CameraOptionsProperty = BindableProperty.Create(nameof(CameraOptions), typeof(CameraOptions), typeof(CameraView), CameraOptions.Default);

		public CameraOptions CameraOptions
		{
			get => (CameraOptions)GetValue(CameraOptionsProperty);
			set => SetValue(CameraOptionsProperty, value);
		}

		public static readonly BindableProperty SavePhotoToFileProperty = BindableProperty.Create(nameof(SavePhotoToFile), typeof(bool), typeof(CameraView), false);

		public bool SavePhotoToFile
		{
			get => (bool)GetValue(SavePhotoToFileProperty);
			set => SetValue(SavePhotoToFileProperty, value);
		}

		public static readonly BindableProperty CaptureOptionsProperty = BindableProperty.Create(nameof(CaptureOptions), typeof(CameraCaptureOptions), typeof(CameraView), CameraCaptureOptions.Default);

		public CameraCaptureOptions CaptureOptions
		{
			get => (CameraCaptureOptions)GetValue(CaptureOptionsProperty);
			set => SetValue(CaptureOptionsProperty, value);
		}

		public static readonly BindableProperty KeepScreenOnProperty = BindableProperty.Create(nameof(KeepScreenOn), typeof(bool), typeof(CameraView), false);

		public bool KeepScreenOn
		{
			get => (bool)GetValue(KeepScreenOnProperty);
			set => SetValue(KeepScreenOnProperty, value);
		}

		public static readonly BindableProperty VideoStabilizationProperty = BindableProperty.Create(nameof(VideoStabilization), typeof(bool), typeof(CameraView), false);

		public bool VideoStabilization
		{
			get => (bool)GetValue(VideoStabilizationProperty);
			set => SetValue(VideoStabilizationProperty, value);
		}

		public static readonly BindableProperty FlashModeProperty = BindableProperty.Create(nameof(FlashMode), typeof(CameraFlashMode), typeof(CameraView), CameraFlashMode.Off);

		public CameraFlashMode FlashMode
		{
			get => (CameraFlashMode)GetValue(FlashModeProperty);
			set => SetValue(FlashModeProperty, value);
		}

		public static readonly BindableProperty PreviewAspectProperty = BindableProperty.Create(nameof(PreviewAspect), typeof(Aspect), typeof(CameraView), Aspect.AspectFit);

		public Aspect PreviewAspect
		{
			get => (Aspect)GetValue(PreviewAspectProperty);
			set => SetValue(PreviewAspectProperty, value);
		}

		public static readonly BindableProperty ZoomProperty = BindableProperty.Create(nameof(Zoom), typeof(float), typeof(CameraView), 1f);

		public float Zoom
		{
			get => (float)GetValue(ZoomProperty);
			set => SetValue(ZoomProperty, value);
		}

		public static readonly BindableProperty MaxZoomProperty = BindableProperty.Create(nameof(MaxZoom), typeof(float), typeof(CameraView), 1f);

		public float MaxZoom
		{
			get => (float)GetValue(MaxZoomProperty);
			set => SetValue(MaxZoomProperty, value);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseMediaCaptured(MediaCapturedEventArgs args) => MediaCaptured?.Invoke(this, args);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseMediaCaptureFailed(string message) => MediaCaptureFailed?.Invoke(this, message);

		public void Shutter() => ShutterClicked?.Invoke(this, EventArgs.Empty);

		readonly Lazy<PlatformConfigurationRegistry<CameraView>> _platformConfigurationRegistry;

		public CameraView()
			=> _platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<CameraView>>(() => new PlatformConfigurationRegistry<CameraView>(this));

		public IPlatformElementConfiguration<T, CameraView> On<T>() where T : IConfigPlatform
			=> _platformConfigurationRegistry.Value.On<T>();
	}

	public class MediaCapturedEventArgs : EventArgs
	{
		public object Data { get; set; }
		public ImageSource Image { get; set; }
		public MediaSource Video { get; set; }
	}

	public enum CameraOptions
	{
		Default,
		Front,
		Back,
		External
	}

	public enum CameraCaptureOptions
	{
		Default,
		Photo,
		Video
	}

	public enum CameraFlashMode
	{
		Off,
		On,
		Auto,
		Torch
	}
}