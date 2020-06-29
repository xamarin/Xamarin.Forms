using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.Capture;
using Windows.Devices.Enumeration;
using Windows.Devices.Lights;
using Windows.System.Display;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Windows.Media.Core;
using System.Linq;
using Windows.Media.Devices;

namespace Xamarin.Forms.Platform.UWP
{
	class CameraViewRenderer: ViewRenderer<CameraView, CaptureElement>
	{
		MediaCapture _mediaCapture;
		bool isPreviewing;
		Lamp flash;
		DisplayRequest _displayRequest = new DisplayRequest();
		LowLagMediaRecording _mediaRecording;
		string _filePath;
		bool _busy;
		VideoStabilizationEffect _videoStabilizationEffect;
		MediaEncodingProfile _encodingProfile;
		VideoEncodingProperties _inputPropertiesBackup;
		VideoEncodingProperties _outputPropertiesBackup;

		public CameraViewRenderer()
		{
			_encodingProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
			Xamarin.Forms.CameraView.VerifyCameraViewFlagEnabled(nameof(CameraViewRenderer));
		}

		bool IsBusy
		{
			get => _busy;
			set
			{
				if (_busy != value)
					Element.IsBusy = _busy = value;
			}
		}

		bool Available
		{
			get => Element?.IsAvailable ?? false;
			set
			{
				if (Element != null && Element.IsAvailable != value)
					Element.IsAvailable = value;
			}
		}

		protected override async void OnElementChanged(ElementChangedEventArgs<CameraView> e)
		{
			Available = false;
			base.OnElementChanged(e);
			if (e.OldElement != null)
			{
				e.OldElement.ShutterClicked += HandleShutter;
			}
			if (e.NewElement != null)
			{
				if (Control != null)
				{
					await CleanupCameraAsync();
					_mediaCapture.Failed -= _mediaCaptureFailed;
				}

				SetNativeControl(new CaptureElement());
				Control.HorizontalAlignment = HorizontalAlignment.Stretch;
				Control.VerticalAlignment = VerticalAlignment.Stretch;

				e.NewElement.ShutterClicked += HandleShutter;

				isPreviewing = false;
				await InitializeCameraAsync();

				if (_mediaCapture != null)
					_mediaCapture.Failed += _mediaCaptureFailed;
			}
		}

		async void HandleShutter(object sender, EventArgs e)
		{
			if (IsBusy)
				return;

			IsBusy = true;
			switch (Element.CaptureOptions)
			{
				default:
				case CameraCaptureOptions.Default:
				case CameraCaptureOptions.Photo:
					if (_mediaRecording != null)
						goto case CameraCaptureOptions.Video;
					var img = await GetImage();
					Element.RaiseMediaCaptured(new MediaCapturedEventArgs
					{
						Image = img
					});
					break;
				case CameraCaptureOptions.Video:
					if (_mediaRecording == null)
						await StartRecord();
					else
					{
						Element.RaiseMediaCaptured(new MediaCapturedEventArgs
						{
							Video = await StopRecord()
						});
					}
					break;
			}
			IsBusy = false;
		}

		async Task<ImageSource> GetImage()
		{
			IsBusy = true;
			var imageProp = ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8);
			var lowLagCapture = await _mediaCapture.PrepareLowLagPhotoCaptureAsync(imageProp);
			var capturedPhoto = await lowLagCapture.CaptureAsync();
			await lowLagCapture.FinishAsync();
			if (Element.SavePhotoToFile)
			{
				var localFolder = Element.OnThisPlatform().GetPhotoFolder();
				var destinationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(localFolder, CreationCollisionOption.OpenIfExists);
				var file = await destinationFolder.CreateFileAsync($"{DateTime.Now.ToString("yyyyddMM_HHmmss")}.jpg", CreationCollisionOption.GenerateUniqueName);
				using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
				{
					BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
					encoder.SetSoftwareBitmap(capturedPhoto.Frame.SoftwareBitmap);
					await encoder.FlushAsync();
				}
			}
			IsBusy = false;
			return ImageSource.FromStream(() => capturedPhoto.Frame.AsStream());
		}

		internal async Task StartRecord()
		{
			var localFolder = Element.OnThisPlatform().GetVideoFolder();
			var destinationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(localFolder, CreationCollisionOption.OpenIfExists);
			var file = await destinationFolder.CreateFileAsync($"{DateTime.Now.ToString("yyyyddMM_HHmmss")}.mp4", CreationCollisionOption.GenerateUniqueName);
			_filePath = file.Path;
			if (Element.VideoStabilization)
			{
				var stabilizerDefinition = new VideoStabilizationEffectDefinition();
				_videoStabilizationEffect = (VideoStabilizationEffect)await _mediaCapture.AddVideoEffectAsync(stabilizerDefinition, MediaStreamType.VideoRecord);
				var recommendation = _videoStabilizationEffect.GetRecommendedStreamConfiguration(_mediaCapture.VideoDeviceController, _encodingProfile.Video);
				if (recommendation.InputProperties != null)
				{
					_inputPropertiesBackup = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoRecord) as VideoEncodingProperties;
					await _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, recommendation.InputProperties);
					await _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, recommendation.InputProperties);
				}
				if (recommendation.OutputProperties != null)
				{
					_outputPropertiesBackup = _encodingProfile.Video;
					_encodingProfile.Video = recommendation.OutputProperties;
				}
				_videoStabilizationEffect.Enabled = true;
			}
			_mediaRecording = await _mediaCapture.PrepareLowLagRecordToStorageFileAsync(_encodingProfile, file);
			await _mediaRecording.StartAsync();
		}

		internal async Task<MediaSource> StopRecord()
		{
			if (_mediaRecording == null)
				return null;

			await _mediaRecording.StopAsync();
			await _mediaRecording.FinishAsync();
			_mediaRecording = null;

			if (_videoStabilizationEffect != null)
			{
				await _mediaCapture.RemoveEffectAsync(_videoStabilizationEffect);
				_videoStabilizationEffect = null;

				if (_inputPropertiesBackup != null)
				{
					await _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, _inputPropertiesBackup);
					_inputPropertiesBackup = null;
				}
				if (_outputPropertiesBackup != null)
				{
					_encodingProfile.Video = _outputPropertiesBackup;
					_outputPropertiesBackup = null;
				}
				_videoStabilizationEffect = null;
			}

			return MediaSource.FromFile(_filePath);
		}

		void _mediaCaptureFailed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
			=> Element?.RaiseMediaCaptureFailed(errorEventArgs.Message);

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(CameraView.CameraOptions):
					await CleanupCameraAsync();
					await InitializeCameraAsync();
					break;
				case nameof(CameraView.FlashMode):
					if (flash != null)
						flash.IsEnabled = Element.FlashMode == CameraFlashMode.Torch || Element.FlashMode == CameraFlashMode.On;
					break;
				case nameof(CameraView.PreviewAspect):
					// TODO
					break;
				case nameof(CameraView.Zoom):
					UpdateZoom();
					break;
			}
			base.OnElementPropertyChanged(sender, e);
		}

		void UpdateZoom()
		{
			var zoomControl = _mediaCapture.VideoDeviceController.ZoomControl;
			if (!zoomControl.Supported)
				return;

			var settings = new ZoomSettings
			{
				Value = Element.Zoom.Clamp(zoomControl.Min, zoomControl.Max),
				Mode = zoomControl.SupportedModes.Contains(ZoomTransitionMode.Smooth) 
					? ZoomTransitionMode.Smooth
					: zoomControl.SupportedModes.First()
			};

			zoomControl.Configure(settings);
		}

		DeviceInformation FilterCamera(DeviceInformationCollection cameraDevices, Windows.Devices.Enumeration.Panel panel)
		{
			foreach (var cam in cameraDevices)
			{
				if (cam.EnclosureLocation?.Panel == panel)
					return cam;
			}
			return null;
		}

		async Task InitializeCameraAsync()
		{
			Available = false;
			if (_mediaCapture != null)
				return;

			var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
			if (cameraDevices.Count == 0)
			{
				Element?.RaiseMediaCaptureFailed("Camera devices not found.");
				return;
			}

			IsBusy = true;
			DeviceInformation device = null;
			switch (Element.CameraOptions)
			{
				default:
				case CameraOptions.Default:
					device = cameraDevices[0];
					break;
				case CameraOptions.Front:
					device = FilterCamera(cameraDevices, Windows.Devices.Enumeration.Panel.Front);
					break;
				case CameraOptions.Back:
					device = FilterCamera(cameraDevices, Windows.Devices.Enumeration.Panel.Back);
					break;
				case CameraOptions.External:
					device = FilterCamera(cameraDevices, Windows.Devices.Enumeration.Panel.Unknown);
					break;
			}
			if (device == null)
			{
				Element?.RaiseMediaCaptureFailed($"{Element.CameraOptions} camera not found.");
				IsBusy = false;
				return;
			}

			var audioDevice = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
			var selectedAudioDevice = audioDevice.Count == 0 ? null : audioDevice[0].Id;

			_mediaCapture = new MediaCapture();
			try
			{
				await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
				{
					VideoDeviceId = device.Id,
					MediaCategory = MediaCategory.Media,
					StreamingCaptureMode = selectedAudioDevice == null ? StreamingCaptureMode.Video : StreamingCaptureMode.AudioAndVideo,
					AudioProcessing = Windows.Media.AudioProcessing.Default,
					AudioDeviceId = selectedAudioDevice
				});
				flash = await Lamp.GetDefaultAsync();
				if (_mediaCapture.VideoDeviceController.ZoomControl.Supported)
					Element.MaxZoom = _mediaCapture.VideoDeviceController.ZoomControl.Max;
				_displayRequest.RequestActive();
				DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
			}
			catch (UnauthorizedAccessException ex)
			{
				Element?.RaiseMediaCaptureFailed($"The app was denied access to the camera or microphone. {ex.Message}");
				IsBusy = false;
				return;
			}

			try
			{
				Control.Source = _mediaCapture;
				await _mediaCapture.StartPreviewAsync();
				isPreviewing = false;
				IsBusy = false;
				Available = true;
			}
			catch (FileLoadException)
			{
				_mediaCapture.CaptureDeviceExclusiveControlStatusChanged += CaptureDeviceExclusiveControlStatusChanged;
			}
		}

		async Task CleanupCameraAsync()
		{
			Available = false;
			IsBusy = true;
			if (_mediaCapture == null)
				return;

			if (isPreviewing)
				await _mediaCapture.StopPreviewAsync();

			if (_mediaRecording != null)
			{
				Element.RaiseMediaCaptured(new MediaCapturedEventArgs
				{
					Video = await StopRecord()
				});
			}

			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				Control.Source = null;
				_displayRequest?.RequestRelease();

				_mediaCapture.CaptureDeviceExclusiveControlStatusChanged -= CaptureDeviceExclusiveControlStatusChanged;
				_mediaCapture.Dispose();
				_mediaCapture = null;
			});
			IsBusy = false;
		}

		protected override async void Dispose(bool disposing)
		{
			await CleanupCameraAsync();
			base.Dispose(disposing);
		}

		async void CaptureDeviceExclusiveControlStatusChanged(MediaCapture sender, MediaCaptureDeviceExclusiveControlStatusChangedEventArgs args)
		{
			if (args.Status == MediaCaptureDeviceExclusiveControlStatus.SharedReadOnlyAvailable)
				Element?.RaiseMediaCaptureFailed("The camera preview can't be displayed because another app has exclusive access");
			else if (args.Status == MediaCaptureDeviceExclusiveControlStatus.ExclusiveControlAvailable && !isPreviewing)
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await _mediaCapture.StartPreviewAsync());
			IsBusy = false;
		}
	}
}
