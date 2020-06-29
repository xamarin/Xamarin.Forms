using System;
using System.Collections.Generic;
using System.IO;

#if __ANDROID_29__
using AndroidX.Core.Content;
using AndroidX.Fragment.App;
#else
using Android.Support.V4.Content;
using Android.Support.V4.App;
#endif

using Android;
using Android.Hardware.Camera2.Params;
using Android.Graphics;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.Media;
using Android.Views;
using Android.Runtime;
using Android.OS;
using AOrientation = Android.Content.Res.Orientation;
using AVideoSource = Android.Media.VideoSource;
using AView = Android.Views.View;
using ASize = Android.Util.Size;
using App = Android.App.Application;
using Env = Android.OS.Environment;
using LayoutParams = Android.Widget.FrameLayout.LayoutParams;

using Java.Lang;
using Java.Util.Concurrent;

using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using System.Threading.Tasks;
using System.Linq;

namespace Xamarin.Forms.Platform.Android.Renderers
{
	class CameraFragment : Fragment, TextureView.ISurfaceTextureListener
	{
		CameraDevice _device;
		CaptureRequest.Builder _sessionBuilder;
		CameraCaptureSession _session;

		AutoFitTextureView _texture;
		ImageReader _photoReader;
		MediaRecorder _mediaRecorder;
		bool audioPermissionsGranted;
		bool cameraPermissionsGranted;
		ASize _previewSize, _videoSize, _photoSize;
		int _sensorOrientation;
		LensFacing _cameraType;

		bool _busy;
		bool _flashSupported;
		bool _stabilizationSupported;
		bool _repeatingIsRunning;
		FlashMode _flashMode;
		string _cameraId;
		string _videoFile;
		Java.Util.Concurrent.Semaphore _captureSessionOpenCloseLock = new Java.Util.Concurrent.Semaphore(1);
		CameraTemplate cameraTemplate;
		HandlerThread _backgroundThread;
		Handler _backgroundHandler = null;

		float _zoom = 1;
		bool _zoomSupported => _maxDigitalZoom != 0;
		float _maxDigitalZoom;
		Rect _activeRect;

		public bool IsRecordingVideo { get; set; }

		bool UseSystemSound { get; set; }

		CameraManager _manager;
		CameraManager Manager => _manager ?? (_manager = (CameraManager)Context.GetSystemService(Context.CameraService));

		MediaActionSound _mediaSound;
		MediaActionSound MediaSound => _mediaSound ?? (_mediaSound = new MediaActionSound());

		TaskCompletionSource<CameraDevice> _initTaskSource;
		TaskCompletionSource<bool> _permissionsRequested;

		bool IsBusy
		{
			get => _device == null || _busy;
			set
			{
				_busy = value;
				if (Element != null)
					Element.IsBusy = value;
			}
		}

		bool Available
		{
			get => Element?.IsAvailable ?? false;
			set
			{
				if (Element?.IsAvailable != value)
					Element.IsAvailable = value;
			}
		}

		public CameraView Element { get; set; }

		public override AView OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.CameraFragment, null);
		}

		public override void OnViewCreated(AView view, Bundle savedInstanceState)
		{
			_texture = view.FindViewById<AutoFitTextureView>(Resource.Id.cameratexture);
		}

		public override async void OnResume()
		{
			base.OnResume();
			StartBackgroundThread();
			if (_texture.IsAvailable)
			{
				UpdateBackgroundColor();
				UpdateCaptureOptions();
				await RetrieveCameraDevice(force: true);
			} 
			else
			{
				_texture.SurfaceTextureListener = this;
			}
		}

		public override void OnPause()
		{
			CloseSession();
			StopBackgroundThread();
			base.OnPause();
		}

		void StartBackgroundThread()
		{
			_backgroundThread = new HandlerThread("CameraBackground");
			_backgroundThread.Start();
			_backgroundHandler = new Handler(_backgroundThread.Looper);
		}

		void StopBackgroundThread()
		{
			if (_backgroundThread == null)
				return;

			_backgroundThread.QuitSafely();
			try
			{
				_backgroundThread.Join();
				_backgroundThread = null;
				_backgroundHandler = null;
			}
			catch (InterruptedException e)
			{
				LogError("BackgroundThread stoping error", e);
			}
		}

		public async Task RetrieveCameraDevice(bool force = false)
		{
			if (Context == null || (!force && _initTaskSource != null))
				return;

			if (_device != null)
				CloseDevice();

			await RequestCameraPermissions();
			if (!cameraPermissionsGranted)
				return;

			if (!_captureSessionOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
				throw new RuntimeException("Time out waiting to lock camera opening.");

			IsBusy = true;
			_cameraId = GetCameraId();

			if (string.IsNullOrEmpty(_cameraId))
			{
				IsBusy = false;
				_captureSessionOpenCloseLock.Release();
				//_texture.ClearCanvas(Element.BackgroundColor.ToAndroid()); // HANG after select valid camera...
				Element.RaiseMediaCaptureFailed($"No {Element.CameraOptions} camera found");
			}
			else
			{
				try
				{
					var characteristics = Manager.GetCameraCharacteristics(_cameraId);
					var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

					_flashSupported = characteristics.Get(CameraCharacteristics.FlashInfoAvailable) == Java.Lang.Boolean.True;
					_stabilizationSupported = false;
					var stabilizationModes = characteristics.Get(CameraCharacteristics.ControlAvailableVideoStabilizationModes);
					if (stabilizationModes != null)
					{
						var modes = (int[])stabilizationModes;
						foreach (var mode in modes)
						{
							if (mode == (int)ControlVideoStabilizationMode.On)
								_stabilizationSupported = true;
						}
					}
					Element.MaxZoom = _maxDigitalZoom = (float)characteristics.Get(CameraCharacteristics.ScalerAvailableMaxDigitalZoom);
					_activeRect = (Rect)characteristics.Get(CameraCharacteristics.SensorInfoActiveArraySize);
					_photoSize = GetMaxSize(map.GetOutputSizes((int)ImageFormatType.Jpeg));
					_videoSize = GetMaxSize(map.GetOutputSizes(Class.FromType(typeof(MediaRecorder))));
					_previewSize = ChooseOptimalSize(
						map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))), 
						_texture.Width, 
						_texture.Height, 
						cameraTemplate == CameraTemplate.Record ? _videoSize : _photoSize);
					_sensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
					_cameraType = (LensFacing)(int)characteristics.Get(CameraCharacteristics.LensFacing);

					if (Resources.Configuration.Orientation == AOrientation.Landscape)
						_texture.SetAspectRatio(_previewSize.Width, _previewSize.Height);
					else
						_texture.SetAspectRatio(_previewSize.Height, _previewSize.Width);

					_initTaskSource = new TaskCompletionSource<CameraDevice>();

					Manager.OpenCamera(
						_cameraId,
						new CameraStateListener
						{
							OnOpenedAction = device =>
							{
								_initTaskSource?.TrySetResult(device);
							},
							OnDisconnectedAction = device =>
							{
								_initTaskSource?.TrySetResult(null);
								CloseDevice(device);
							},
							OnErrorAction = (device, error) =>
							{
								_initTaskSource?.TrySetResult(device);
								Element?.RaiseMediaCaptureFailed($"Camera device error: {error}");
								CloseDevice(device);
							},
							OnClosedAction = device =>
							{
								_initTaskSource?.TrySetResult(null);
								CloseDevice(device);
							}
						},
						_backgroundHandler);

					_captureSessionOpenCloseLock.Release();
					_device = await _initTaskSource.Task;
					_initTaskSource = null;
					if (_device != null) 
					{
						await PrepareSession();
					}
				}
				catch (Java.Lang.Exception error)
				{
					LogError("Failed to open camera", error);
					Available = false;
				}
				finally
				{
					IsBusy = false;
				}
			}
		}

		public void UpdateCaptureOptions()
		{ 
			switch (Element.CaptureOptions)
			{
				default:
				case CameraCaptureOptions.Photo:
					cameraTemplate = CameraTemplate.Preview;
					break;
				case CameraCaptureOptions.Video:
					cameraTemplate = CameraTemplate.Record;
					break;
			}
		}

		public void TakePhoto()
		{
			if (IsBusy || cameraTemplate != CameraTemplate.Preview)
				return;

			try
			{
				if (_device != null)
				{
					_session.StopRepeating();
					_repeatingIsRunning = false;
					_sessionBuilder.AddTarget(_photoReader.Surface);
					_sessionBuilder.Set(CaptureRequest.FlashMode, (int)_flashMode);
					_session.Capture(_sessionBuilder.Build(), null, null);
					_sessionBuilder.RemoveTarget(_photoReader.Surface);
					UpdateRepeatingRequest();
				}
			}
			catch (Java.Lang.Exception error)
			{
				LogError("Failed to take photo", error);
			}
		}

		void OnPhoto(object sender, byte[] data)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				Element?.RaiseMediaCaptured(new MediaCapturedEventArgs()
				{
					Data = data,
					Image = ImageSource.FromStream(() => new MemoryStream(data))
				});
			});
		}

		void OnVideo(object sender, string data)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				Element?.RaiseMediaCaptured(new MediaCapturedEventArgs()
				{
					Video = MediaSource.FromFile(data)
				});
			});
		}

		void SetupImageReader()
		{
			DisposeImageReader();

			_photoReader = ImageReader.NewInstance(640, 480, ImageFormatType.Jpeg, maxImages: 1);

			var readerListener = new ImageAvailableListener();
			readerListener.Photo += (_, bytes) =>
			{
				if (Element.SavePhotoToFile)
					File.WriteAllBytes(ConstructMediaFilename(null, "jpg"), bytes);
				Sound(MediaActionSoundType.ShutterClick);
				OnPhoto(this, bytes);
			};

			_photoReader.SetOnImageAvailableListener(readerListener, _backgroundHandler);
		}

		void SetupMediaRecorder(Surface previewSurface)
		{
			DisposeMediaRecorder();

			_mediaRecorder = new MediaRecorder();
			_mediaRecorder.SetPreviewDisplay(previewSurface);
			if (audioPermissionsGranted)
				_mediaRecorder.SetAudioSource(AudioSource.Camcorder);
			_mediaRecorder.SetVideoSource(AVideoSource.Surface);

			var profile = GetCamcoderProfile();
			if (profile != null)
			{
				_mediaRecorder.SetProfile(profile);
			}
			else
			{
				_mediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
				_mediaRecorder.SetVideoEncodingBitRate(10000000);
				_mediaRecorder.SetVideoFrameRate(30);
				_mediaRecorder.SetVideoSize(_videoSize.Width, _videoSize.Height);
				_mediaRecorder.SetVideoEncoder(VideoEncoder.H264);
				if (audioPermissionsGranted)
				{
					_mediaRecorder.SetAudioEncoder(AudioEncoder.Default);
				}
			}

			_videoFile = ConstructMediaFilename("VID", "mp4");

			_mediaRecorder.SetOutputFile(_videoFile);
			_mediaRecorder.SetOrientationHint(GetCaptureOrientation());
			_mediaRecorder.Prepare();
		}

		CamcorderProfile GetCamcoderProfile()
		{
			var cameraId = Convert.ToInt32(_cameraId);
			if (CamcorderProfile.HasProfile(cameraId, CamcorderQuality.HighSpeed1080p))
				return CamcorderProfile.Get(cameraId, CamcorderQuality.HighSpeed1080p);
			else if (CamcorderProfile.HasProfile(cameraId, CamcorderQuality.HighSpeed720p))
				return CamcorderProfile.Get(cameraId, CamcorderQuality.HighSpeed720p);
			else if (CamcorderProfile.HasProfile(cameraId, CamcorderQuality.HighSpeed480p))
				return CamcorderProfile.Get(cameraId, CamcorderQuality.HighSpeed480p);
			else if (CamcorderProfile.HasProfile(cameraId, CamcorderQuality.High))
				return CamcorderProfile.Get(cameraId, CamcorderQuality.High);
			else if (CamcorderProfile.HasProfile(cameraId, CamcorderQuality.Low))
				return CamcorderProfile.Get(cameraId, CamcorderQuality.Low);

			return null;
		}

		public void StartRecord()
		{
			if (IsBusy)
			{
				return;
			}
			else if (IsRecordingVideo)
			{
				Element?.RaiseMediaCaptureFailed("Video already recording.");
				return;
			}
			else if (cameraTemplate != CameraTemplate.Record)
			{
				Element?.RaiseMediaCaptureFailed($"Unexpected error: Camera {cameraTemplate} not configured to record video.");
				return;
			}
			else if (_mediaRecorder == null)
			{
				Element?.RaiseMediaCaptureFailed($"Unexpected error: MediaRecorder is not initialized.");
				IsRecordingVideo = false;
				return;
			}

			try
			{
				Sound(MediaActionSoundType.StartVideoRecording);
				_mediaRecorder.Start();
				IsRecordingVideo = true;
			}
			catch (Java.Lang.Exception error)
			{
				LogError("Failed to take video", error);
				Element?.RaiseMediaCaptureFailed($"Failed to take video: {error}");
				DisposeMediaRecorder();
			}
		}

		public async void StopRecord()
		{
			if (IsBusy || !IsRecordingVideo || _session == null || _mediaRecorder == null)
				return;

			try
			{
				DisposeMediaRecorder();
				await PrepareSession();
			}
			catch (Java.Lang.Exception ex)
			{
				LogError("Stop record exception", ex);
			}
			finally
			{
				IsRecordingVideo = false;
			}

			Sound(MediaActionSoundType.StopVideoRecording);
			OnVideo(this, _videoFile);
		}

		async Task PrepareSession()
		{
			IsBusy = true;
			try
			{
				CloseSession();

				_sessionBuilder = _device.CreateCaptureRequest(cameraTemplate);

				SetFlash();
				SetVideoStabilization();
				ApplyZoom();

				var surfaces = new List<Surface>();

				// preview texture
				if (_texture.IsAvailable && _previewSize != null)
				{
					var texture = _texture.SurfaceTexture;
					texture.SetDefaultBufferSize(_previewSize.Width, _previewSize.Height);
					var previewSurface = new Surface(texture);
					surfaces.Add(previewSurface);
					_sessionBuilder.AddTarget(previewSurface);

					// video mode
					if (cameraTemplate == CameraTemplate.Record)
					{
						SetupMediaRecorder(previewSurface);
						var _mediaSurface = _mediaRecorder.Surface;
						surfaces.Add(_mediaSurface);
						_sessionBuilder.AddTarget(_mediaSurface);
					}
					// photo mode
					else
					{
						SetupImageReader();
						surfaces.Add(_photoReader.Surface);
					}
				}

				var tcs = new TaskCompletionSource<CameraCaptureSession>();

				_device.CreateCaptureSession(
					surfaces,
					new CameraCaptureStateListener()
					{
						OnConfigureFailedAction = session =>
						{
							tcs.SetResult(null);
							Element.RaiseMediaCaptureFailed("Failed to create captire sesstion");
						},
						OnConfiguredAction = session =>
						{
							tcs.SetResult(session);
						}
					},
					null);

				_session = await tcs.Task;
				if (_session != null)
				{
					UpdateRepeatingRequest();
				}
			}
			catch (Java.Lang.Exception error)
			{
				Available = false;
				LogError("Capture", error);
			}
			finally
			{
				Available = _session != null;
				IsBusy = false;
			}
		}

		void CloseSession()
		{
			_repeatingIsRunning = false;

			if (_session == null)
				return;

			try
			{
				_session.StopRepeating();
				_session.AbortCaptures();
				_session.Close();
				_session.Dispose();
				_session = null;
			}
			catch (CameraAccessException e)
			{
				LogError("Error camera access", e);
			}
			catch (Java.Lang.Exception e)
			{
				LogError("Error close device", e);
			}
		}

		public void UpdateRepeatingRequest()
		{
			if (_session == null || _sessionBuilder == null)
				return;

			IsBusy = true;
			try
			{
				if (_repeatingIsRunning)
					_session.StopRepeating();

				_sessionBuilder.Set(CaptureRequest.ControlMode, (int)ControlMode.Auto);
				_sessionBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.On);
				if (cameraTemplate == CameraTemplate.Record)
					_sessionBuilder.Set(CaptureRequest.FlashMode, (int)_flashMode);

				_session.SetRepeatingRequest(_sessionBuilder.Build(), null, _backgroundHandler);
				_repeatingIsRunning = true;
			}
			catch (Java.Lang.Exception error)
			{
				LogError("Update preview exception.", error);
			}
			finally
			{
				IsBusy = false;
			}
		}

		void CloseDevice(CameraDevice inputDevice)
		{
			if (inputDevice == _device)
				CloseDevice();
		}

		public void CloseDevice()
		{
			try
			{
				DisposeMediaRecorder();
			}
			catch
			{
			}
			CloseSession();
			try
			{
				if (_sessionBuilder != null)
				{
					_sessionBuilder.Dispose();
					_sessionBuilder = null;
				}

				if (_device != null)
				{
					_device.Close();
					_device = null;
				}

				DisposeImageReader();
			}
			catch (Java.Lang.Exception e)
			{
				LogError("Error close device", e);
			}
		}

		void UpdateBackgroundColor()
		{
			View?.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
		}

		public void SetFlash()
		{
			if (!_flashSupported)
				return;

			switch (Element.FlashMode)
			{
				default:
				case CameraFlashMode.On:
				case CameraFlashMode.Auto:
					_flashMode = FlashMode.Single;
					break;
				case CameraFlashMode.Off:
					_flashMode = FlashMode.Off;
					break;
				case CameraFlashMode.Torch:
					_flashMode = FlashMode.Torch;
					break;
			}
		}

		public void SetVideoStabilization()
		{
			if (_sessionBuilder == null || !_stabilizationSupported)
			{
				_sessionBuilder.Set(CaptureRequest.ControlVideoStabilizationMode,
					(int)(Element.VideoStabilization ? ControlVideoStabilizationMode.On : ControlVideoStabilizationMode.Off));
			}
		}

		public void ApplyZoom()
		{
			_zoom = System.Math.Max(1f, System.Math.Min(Element.Zoom, _maxDigitalZoom));
			if (_zoomSupported)
				_sessionBuilder?.Set(CaptureRequest.ScalerCropRegion, GetZoomRect());
		}

		string GetCameraId()
		{
			var cameraIdList = Manager.GetCameraIdList();
			if (cameraIdList.Length == 0)
				return null;

			string FilterCameraByLens(LensFacing lensFacing)
			{
				foreach (var id in cameraIdList)
				{
					var characteristics = Manager.GetCameraCharacteristics(id);
					if (lensFacing == (LensFacing)(int)characteristics.Get(CameraCharacteristics.LensFacing))
						return id;
				}
				return null;
			}

			switch (Element.CameraOptions)
			{
				default:
				case CameraOptions.Default:
					return cameraIdList.Length != 0 ? cameraIdList[0] : null;
				case CameraOptions.Front:
					return FilterCameraByLens(LensFacing.Front);
				case CameraOptions.Back:
					return FilterCameraByLens(LensFacing.Back);
				case CameraOptions.External:
					return FilterCameraByLens(LensFacing.External);
			}
		}

		#region TextureView.ISurfaceTextureListener
		async void TextureView.ISurfaceTextureListener.OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
		{
			UpdateBackgroundColor();
			UpdateCaptureOptions();
			await RetrieveCameraDevice();
		}

		void TextureView.ISurfaceTextureListener.OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{
			ConfigureTransform(width, height);
		}

		bool TextureView.ISurfaceTextureListener.OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			CloseDevice();
			return true;
		}

		void TextureView.ISurfaceTextureListener.OnSurfaceTextureUpdated(SurfaceTexture surface)
		{
		}
		#endregion

		#region Permissions
		async Task RequestCameraPermissions()
		{
			if (_permissionsRequested != null)
				await _permissionsRequested.Task;

			cameraPermissionsGranted = ContextCompat.CheckSelfPermission(Context, Manifest.Permission.Camera) == Permission.Granted;
			audioPermissionsGranted = ContextCompat.CheckSelfPermission(Context, Manifest.Permission.RecordAudio) == Permission.Granted;
			if (!cameraPermissionsGranted || !audioPermissionsGranted)
			{
				_permissionsRequested = new TaskCompletionSource<bool>();
				RequestPermissions(new[] { Manifest.Permission.Camera, Manifest.Permission.RecordAudio }, requestCode: 1);
				await _permissionsRequested.Task;
				_permissionsRequested = null;
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			if (requestCode != 1)
			{
				base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
				return;
			}
			for (int i = 0; i < permissions.Length; i++)
			{
				if (permissions[i] == Manifest.Permission.Camera)
				{
					cameraPermissionsGranted = grantResults[i] == Permission.Granted;
					if (!cameraPermissionsGranted)
						Element.RaiseMediaCaptureFailed($"No permission to use the camera.");
				}
				else if (permissions[i] == Manifest.Permission.RecordAudio)
				{
					audioPermissionsGranted = grantResults[i] == Permission.Granted;
					if (!audioPermissionsGranted)
					{
						Element.RaiseMediaCaptureFailed($"No permission to record audio.");
					}
				}
			}
			_permissionsRequested?.TrySetResult(true);
		}
		#endregion

		#region Helpers
		void LogError(string desc, Java.Lang.Exception ex = null)
		{
			var newLine = System.Environment.NewLine;
			var sb = new StringBuilder(desc);
			if (ex != null)
			{
				sb.Append($"{newLine}ErrorMessage: {ex.Message}{newLine}Stacktrace: {ex.StackTrace}");
				ex.PrintStackTrace();
			}
			Log.Warning("CameraView", sb.ToString());
		}

		void DisposeMediaRecorder()
		{
			if (_mediaRecorder != null)
			{
				if (IsRecordingVideo)
				{
					_mediaRecorder.Stop();
					_mediaRecorder.Reset();
				}
				_mediaRecorder.Release();
				_mediaRecorder.Dispose();
				_mediaRecorder = null;
			}
			IsRecordingVideo = false;
		}

		void DisposeImageReader()
		{
			if (_photoReader != null)
			{
				_photoReader.Close();
				_photoReader.Dispose();
				_photoReader = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			CloseDevice();
			base.Dispose(disposing);
		}

		string ConstructMediaFilename(string prefix, string extension)
		{
			// "To improve user privacy, direct access to shared/external storage devices is deprecated"
			// Env.GetExternalStoragePublicDirectory(Env.DirectoryDcim).AbsolutePath
			var path = Context.GetExternalFilesDir(Env.DirectoryDcim).AbsolutePath;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			var fileName = DateTime.Now.ToString("yyyyddMM_HHmmss");
			if (!string.IsNullOrEmpty(prefix))
				fileName = $"{prefix}_{fileName}";
			return System.IO.Path.Combine(path, $"{fileName}.{extension}");
		}

		Rect GetZoomRect()
		{
			if (_activeRect == null)
				return null;
			var width = _activeRect.Width();
			var heigth = _activeRect.Height();
			var newWidth = (int)(width / _zoom);
			var newHeight = (int)(heigth / _zoom);
			var x = (width - newWidth) / 2;
			var y = (heigth - newHeight) / 2;
			return new Rect(x, y, x + newWidth, y + newHeight);
		}

		SurfaceOrientation GetDisplayRotation()
			=> App.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay.Rotation;

		int GetDisplayRotationiDegress
		{
			get
			{
				switch (GetDisplayRotation())
				{
					case SurfaceOrientation.Rotation90:
						return 90;
					case SurfaceOrientation.Rotation180:
						return 180;
					case SurfaceOrientation.Rotation270:
						return 270;
					default:
						return 0;
				}
			}
		}

		public bool KeepScreenOn {
			get => _texture.KeepScreenOn;
			set => _texture.KeepScreenOn = value;
		}

		int GetPreviewOrientation()
		{
			switch (GetDisplayRotation())
			{
				case SurfaceOrientation.Rotation90:
					return 270;
				case SurfaceOrientation.Rotation180:
					return 180;
				case SurfaceOrientation.Rotation270:
					return 90;
				default:
					return 0;
			}
		}

		public void ConfigureTransform()
		{
			ConfigureTransform(_texture.Width, _texture.Height);
		}

		void ConfigureTransform(int viewWidth, int viewHeight)
		{
			if (_texture == null || _previewSize == null || _previewSize.Width == 0 || _previewSize.Height == 0)
				return;

			var matrix = new Matrix();
			var viewRect = new RectF(0, 0, viewWidth, viewHeight);
			var bufferRect = new RectF(0, 0, _previewSize.Height, _previewSize.Width);
			var centerX = viewRect.CenterX();
			var centerY = viewRect.CenterY();
			bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());

			var mirror = Element.CameraOptions == CameraOptions.Front && Element.OnThisPlatform().GetMirrorFrontPreview();
			matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
			float scaleHH() => (float)viewHeight / _previewSize.Height;
			float scaleHW() => (float)viewHeight / _previewSize.Width;
			float scaleWW() => (float)viewWidth / _previewSize.Width;
			float scaleWH() => (float)viewWidth / _previewSize.Height;
			float sx, sy;

			switch (Element.PreviewAspect)
			{
				default:
				case Aspect.AspectFit:
					sx = sy = System.Math.Min(scaleHH(), scaleHW());
					break;
				case Aspect.AspectFill:
					sx = sy = System.Math.Max(scaleHH(), scaleHW());
					break;
				case Aspect.Fill:
					if (Resources.Configuration.Orientation == AOrientation.Landscape)
					{
						sx = scaleWW();
						sy = scaleHH();
					}
					else
					{
						sx = scaleWH();
						sy = scaleHW();
					}
					break;
			}

			matrix.PostScale(mirror ? -sx : sx, sy, centerX, centerY);
			//matrix.PostRotate(90 * ((int)GetDisplayRotation() - 2), centerX, centerY);
			_texture.SetTransform(matrix);
		}

		int GetCaptureOrientation()
		{
			if (_cameraType == LensFacing.Front)
			{
				var frontResult = (_sensorOrientation + GetDisplayRotationiDegress) % 360;
				return (360 - frontResult) % 360;
			}

			var result = _sensorOrientation - GetDisplayRotationiDegress;
			return (result + 360) % 360;
		}

		void Sound(MediaActionSoundType soundType)
		{
			if (UseSystemSound)
				_mediaSound.Play(soundType);
		}

		ASize GetMaxSize(ASize[] ImageSizes)
		{
			ASize maxSize = null;
			long maxPixels = 0;
			for (int i = 0; i < ImageSizes.Length; i++)
			{
				long currentPixels = ImageSizes[i].Width * ImageSizes[i].Height;
				if (currentPixels > maxPixels)
				{
					maxSize = ImageSizes[i];
					maxPixels = currentPixels;
				}
			}
			return maxSize;
		}

		// chooses the smallest one whose width and height are at least as large as the respective requested values
		ASize ChooseOptimalSize(ASize[] choices, int width, int height, ASize aspectRatio)
		{
			List<ASize> bigEnough = new List<ASize>();
			int w = aspectRatio.Width;
			int h = aspectRatio.Height;
			foreach (var option in choices)
			{
				if (option.Height == option.Width * h / w &&
						option.Width >= width && option.Height >= height)
				{
					bigEnough.Add(option);
				}
			}
			// Pick the smallest of those, assuming we found any
			if (bigEnough.Count > 0)
			{
				var minArea = bigEnough.Min(s => s.Width * s.Height);
				return bigEnough.First(s => s.Width * s.Height == minArea);
			}
			else
			{
				LogError("Couldn't find any suitable preview size");
				return choices[0];
			}
		}
		#endregion
	}
}
