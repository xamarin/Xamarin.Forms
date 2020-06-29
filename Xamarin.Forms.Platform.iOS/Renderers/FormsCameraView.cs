using System;
using System.IO;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using AVFoundation;
using CoreGraphics;
using CoreMedia;

using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public sealed class FormsCameraView : UIView, IAVCaptureFileOutputRecordingDelegate
	{
		readonly AVCaptureVideoPreviewLayer _previewLayer;
		readonly AVCaptureSession _captureSession;
		UIView _mainView;
		AVCaptureDeviceInput _input;
		AVCaptureStillImageOutput _imageOutput;
		AVCapturePhotoOutput _photoOutput;
		AVCaptureMovieFileOutput _videoOutput;
		AVCaptureConnection _captureConnection;
		AVCaptureDevice _device;
		bool _isBusy;
		bool _isAvailable;
		CameraFlashMode _flashMode;
		readonly float _imgScale = 1f;

		public event EventHandler<bool> Busy;
		public event EventHandler<bool> Available;
		public event EventHandler<Tuple<NSObject, NSError>> FinishCapture;

		public bool VideoRecorded => _videoOutput?.Recording == true;

		public FormsCameraView()
		{
			_flashMode = CameraFlashMode.Off;
			_mainView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
			AutoresizingMask = UIViewAutoresizing.FlexibleMargins;

			_captureSession = new AVCaptureSession
			{
				SessionPreset = AVCaptureSession.PresetHigh
			};

			_previewLayer = new AVCaptureVideoPreviewLayer(_captureSession)
			{
				VideoGravity = AVLayerVideoGravity.ResizeAspectFill
			};

			_mainView.Layer.AddSublayer(_previewLayer);
			RetrieveCameraDevice(CameraOptions.Default);

			Add(_mainView);

			AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[mainView]|", NSLayoutFormatOptions.DirectionLeftToRight, null, new NSDictionary("mainView", _mainView)));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[mainView]|", NSLayoutFormatOptions.AlignAllTop, null, new NSDictionary("mainView", _mainView)));
		}

		void SetStartOrientation()
		{
			CGRect previewLayerFrame = _previewLayer.Frame;

			switch (UIApplication.SharedApplication.StatusBarOrientation)
			{
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					previewLayerFrame.Height = UIScreen.MainScreen.Bounds.Height;
					previewLayerFrame.Width = UIScreen.MainScreen.Bounds.Width;
					break;

				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
					previewLayerFrame.Width = UIScreen.MainScreen.Bounds.Width;
					previewLayerFrame.Height = UIScreen.MainScreen.Bounds.Height;
					break;
			}

			try
			{
				_previewLayer.Frame = previewLayerFrame;
			}
			catch (Exception error)
			{
				LogError("Failed to adjust frame", error);
			}
		}

		void LogError(string message, Exception error = null)
		{
			var errorMessage = error == null
				? string.Empty
				: Environment.NewLine +
					$"ErrorMessage: {Environment.NewLine}" +
					error.Message + Environment.NewLine +
					$"Stacktrace: {Environment.NewLine}" +
					error.StackTrace;

			Log.Warning("Camera", $"{message}{errorMessage}");
		}

		bool IsBusy
		{
			get => _isBusy;
			set
			{
				if (_isBusy != value)
					Busy?.Invoke(this, value);
				_isBusy = value;
			}
		}

		UIImage RotateImage(UIImage image)
		{
			CGImage imgRef = image.CGImage;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();

			var imgHeight = imgRef.Height * _imgScale;
			var imgWidth = imgRef.Width * _imgScale;

			CGRect bounds = new CGRect(0, 0, imgWidth, imgHeight);
			CGSize imageSize = new CGSize(imgWidth, imgHeight);
			UIImageOrientation orient = image.Orientation;

			switch (orient)
			{
				case UIImageOrientation.Up:
					transform = CGAffineTransform.MakeIdentity();
					break;
				case UIImageOrientation.Down:
					transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
					break;
				case UIImageOrientation.Right:
					bounds.Size = new CGSize(bounds.Size.Height, bounds.Size.Width);
					transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
					break;
				default:
					throw new Exception("Invalid image orientation");
			}

			UIGraphics.BeginImageContext(bounds.Size);
			CGContext context = UIGraphics.GetCurrentContext();

			if (orient == UIImageOrientation.Right)
			{
				context.ScaleCTM(-1, 1);
				context.TranslateCTM(-imgHeight, 0);
			}
			else
			{
				context.ScaleCTM(1, -1);
				context.TranslateCTM(0, -imgHeight);
			}

			context.ConcatCTM(transform);

			context.DrawImage(new CGRect(0, 0, imgWidth, imgHeight), imgRef);
			image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return image;
		}

		public override void Draw(CGRect rect)
		{
			_previewLayer.Frame = rect;
			base.Draw(rect);
		}

		public float Zoom
		{
			get => (float)(_device?.VideoZoomFactor ?? 1f);
			set
			{
				if (_device == null)
					return;
				_device.LockForConfiguration(out NSError err);
				_device.VideoZoomFactor = (nfloat)Math.Max(1, Math.Min(value, MaxZoom));
				_device.UnlockForConfiguration();
			}
		}

		public float MaxZoom => (float)(_device?.ActiveFormat.VideoMaxZoomFactor ?? 1f);

		public async Task TakePhoto()
		{
			if (_isBusy || _device == null || _videoOutput != null)
				return;

			IsBusy = true;
			// iOS >= 10
			if (_photoOutput != null)
			{
				var photoOutputConnection = _photoOutput.ConnectionFromMediaType(AVMediaType.Video);
				if (photoOutputConnection != null)
					photoOutputConnection.VideoOrientation = _previewLayer.Connection.VideoOrientation;

				var photoSettings = AVCapturePhotoSettings.Create();
				photoSettings.FlashMode = (AVCaptureFlashMode)_flashMode;
				photoSettings.IsHighResolutionPhotoEnabled = true;

				var photoCaptureDelegate = new PhotoCaptureDelegate
				{
					OnFinishCapture = (data, error) =>
					{
						FinishCapture?.Invoke(this, new Tuple<NSObject, NSError>(data, error));
						IsBusy = false;
					},
					WillCapturePhotoAnimation = () => Animate(0.25, () => _previewLayer.Opacity = 1)
				};

				_photoOutput.CapturePhoto(photoSettings, photoCaptureDelegate);
				return;
			}
			// iOS < 10
			try
			{
				var connection = _imageOutput.Connections[0];
				connection.VideoOrientation = _previewLayer.Connection.VideoOrientation;
				var sampleBuffer = await _imageOutput.CaptureStillImageTaskAsync(connection);
				var imageData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
				FinishCapture?.Invoke(this, new Tuple<NSObject, NSError>(imageData, null));
			}
			catch (Exception)
			{
				FinishCapture?.Invoke(this, new Tuple<NSObject, NSError>(null, new NSError(new NSString("faled create image"), 0)));
			}
			IsBusy = false;
		}

		string ConstructVideoFilename()
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var library = Path.Combine(documents, "..", "Library");
			var timeStamp = DateTime.Now.ToString("yyyyddMM_HHmmss");
			return Path.Combine(library, $"VID_{timeStamp}.mov");
		}

		public void StartRecord() // TODO audio record
		{
			if (_isBusy || _device == null || _videoOutput?.Recording == true)
				return;

			_captureSession.BeginConfiguration();

			_videoOutput = new AVCaptureMovieFileOutput();
			if (_captureSession.CanAddOutput(_videoOutput))
				_captureSession.AddOutput(_videoOutput);

			_captureSession.CommitConfiguration();

			IsBusy = true;
			try
			{
				_videoOutput.Connections[0].VideoOrientation = _previewLayer.Connection.VideoOrientation;
				var connection = _videoOutput.Connections[0];

				if (connection.SupportsVideoOrientation)
					connection.VideoOrientation = _previewLayer.Orientation;
				if (connection.SupportsVideoStabilization)
					connection.PreferredVideoStabilizationMode = VideoStabilization ? AVCaptureVideoStabilizationMode.Auto : AVCaptureVideoStabilizationMode.Off;

				var outputFileURL = NSUrl.FromFilename(ConstructVideoFilename());

				_videoOutput.StartRecordingToOutputFile(outputFileURL, this);
			}
			catch (Exception error)
			{
				LogError("Error with camera output capture", error);
			}
			IsBusy = false;
		}

		public void StopRecord()
		{
			if (!_isBusy && _device != null && _videoOutput != null && _videoOutput.Recording)
			{
				IsBusy = true;
				_videoOutput.StopRecording();
			}
		}

		public void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			FinishCapture?.Invoke(this, new Tuple<NSObject, NSError>(outputFileUrl, error));
			_captureSession.RemoveOutput(_videoOutput);
			_videoOutput = null;
			IsBusy = false;
		}

		public void SwitchFlash(CameraFlashMode newFlashMode)
		{
			if (_isAvailable && _device != null && newFlashMode != _flashMode)
			{
				_flashMode = newFlashMode;
				SwitchFlash();
			}
		}

		void SwitchFlash()
		{
			try
			{
				_device.LockForConfiguration(out NSError err);

				switch (_flashMode)
				{
					default:
					case CameraFlashMode.Off:
						if (_device.IsFlashModeSupported(AVCaptureFlashMode.Off))
							_device.FlashMode = AVCaptureFlashMode.Off;
						break;
					case CameraFlashMode.On:
						if (_device.IsFlashModeSupported(AVCaptureFlashMode.On))
							_device.FlashMode = AVCaptureFlashMode.On;
						break;
					case CameraFlashMode.Auto:
						if (_device.IsFlashModeSupported(AVCaptureFlashMode.Auto))
							_device.FlashMode = AVCaptureFlashMode.Auto;
						break;
					case CameraFlashMode.Torch:
						if (_device.IsTorchModeSupported(AVCaptureTorchMode.On))
							_device.TorchMode = AVCaptureTorchMode.On;
						break;
				}

				if (_flashMode != CameraFlashMode.Torch && 
					_device.TorchMode == AVCaptureTorchMode.On && 
					_device.IsTorchModeSupported(AVCaptureTorchMode.Off))
					_device.TorchMode = AVCaptureTorchMode.Off;

				_device.UnlockForConfiguration();
			}
			catch (Exception error)
			{
				LogError("Failed to switch flash on/off", error);
			}
		}

		public bool VideoStabilization { get; set; }

		public void SetBounds(double width, double height)
		{
			_mainView.Frame = new CGRect(0, 0, width, height);
			Draw(_mainView.Frame);
		}

		public void ChangeFocusPoint(Point point)
		{
			if (!_isAvailable && _device == null)
				return;

			try
			{
				_device.LockForConfiguration(out NSError err);

				var focus_x = point.X / Bounds.Width;
				var focus_y = point.Y / Bounds.Height;

				if (_device.FocusPointOfInterestSupported)
					_device.FocusPointOfInterest = new CGPoint(focus_x, focus_y);
				if (_device.ExposurePointOfInterestSupported)
					_device.ExposurePointOfInterest = new CGPoint(focus_x, focus_y);

				_device.UnlockForConfiguration();
			}
			catch (Exception error)
			{
				LogError("Failed to adjust focus" , error);
			}
		}

		public void RetrieveCameraDevice(CameraOptions cameraOptions)
		{
			bool cameraAccess = false;
			switch (AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video))
			{
				case AVAuthorizationStatus.Authorized:
					cameraAccess = true;
					break;
				case AVAuthorizationStatus.NotDetermined:
					AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, granted => cameraAccess = granted);
					break;
			}

			if (!cameraAccess)
				return;

			AVCaptureDevicePosition position;
			switch (cameraOptions)
			{
				default:
				case CameraOptions.Default:
				case CameraOptions.Back:
					position = AVCaptureDevicePosition.Back; break;
				case CameraOptions.Front:
					position = AVCaptureDevicePosition.Front; break;
				case CameraOptions.External:
					position = AVCaptureDevicePosition.Unspecified; break;
			}

			_device = null;
			var devs = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
			foreach (var d in devs)
			{
				if (d.Position == position)
					_device = d;
			}

			if (_device == null)
			{
				ClearCaptureSession();
				_isAvailable = false;
				LogError("No device detected");
				return;
			}
			_isAvailable = _device != null;

			InitializeCamera();
			SwitchFlash();
		}

		void InitializeCamera()
		{
			if (_device == null)
			{
				LogError("Camera failed to initialise.");
				return;
			}

			try
			{
				_device.LockForConfiguration(out NSError err);

				if (_device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
					_device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;

				_device.UnlockForConfiguration();

				ClearCaptureSession();

				_input = new AVCaptureDeviceInput(_device, out NSError error);
				if (error != null)
					LogError($"Could not create device input: {error.LocalizedDescription}");

				_captureSession.BeginConfiguration();

				if (_captureSession.CanAddInput(_input))
					_captureSession.AddInput(_input);
				if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
				{
					_photoOutput = new AVCapturePhotoOutput();
					if (_captureSession.CanAddOutput(_photoOutput))
					{
						_captureSession.AddOutput(_photoOutput);
						_photoOutput.IsHighResolutionCaptureEnabled = true;
						_photoOutput.IsLivePhotoCaptureEnabled = _photoOutput.IsLivePhotoCaptureSupported;
					}
				}
				else 
				{
					_imageOutput = new AVCaptureStillImageOutput();
					if (_captureSession.CanAddOutput(_imageOutput))
							_captureSession.AddOutput(_imageOutput);
				}

				_captureSession.CommitConfiguration();

				InvokeOnMainThread(() => 
				{
					_captureConnection = _previewLayer.Connection;
					SetStartOrientation();
					_captureSession.StartRunning();
				});
			}
			catch (Exception error)
			{
				LogError("Camera failed to initialise", error);
			}

			Draw(_mainView.Frame);
			Available?.Invoke(this, _isAvailable);
		}

		void ClearCaptureSession()
		{
			if (_captureSession != null)
			{
				if (_captureSession.Running)
					_captureSession.StopRunning();
				if (_imageOutput != null)
					_captureSession.RemoveOutput(_imageOutput);
				if (_photoOutput != null)
					_captureSession.RemoveOutput(_photoOutput);
				if (_videoOutput != null)
					_captureSession.RemoveOutput(_videoOutput);
				if (_input != null)
					_captureSession.RemoveInput(_input);
			}
			_input?.Dispose();
			_imageOutput?.Dispose();
			_photoOutput?.Dispose();
			_videoOutput?.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			if (_device?.TorchMode == AVCaptureTorchMode.On)
			{
				_flashMode = CameraFlashMode.Off;
				SwitchFlash();
			}

			ClearCaptureSession();
			base.Dispose(disposing);
		}
	}

	class PhotoCaptureDelegate : NSObject, IAVCapturePhotoCaptureDelegate
	{
		public Action<NSData, NSError> OnFinishCapture;
		public Action WillCapturePhotoAnimation;

		NSData photoData;

		[Export("captureOutput:willCapturePhotoForResolvedSettings:")]
		public void WillCapturePhoto(AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings) => WillCapturePhotoAnimation();

		[Export("captureOutput:didFinishProcessingPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		public void DidFinishProcessingPhoto(AVCapturePhotoOutput captureOutput, CMSampleBuffer photoSampleBuffer, CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, AVCaptureBracketedStillImageSettings bracketSettings, NSError error)
		{
			if (photoSampleBuffer != null)
				photoData = AVCapturePhotoOutput.GetJpegPhotoDataRepresentation(photoSampleBuffer, previewPhotoSampleBuffer);
			else
				Console.WriteLine($"Error capturing photo: {error.LocalizedDescription}");
		}

		[Export("captureOutput:didFinishCaptureForResolvedSettings:error:")]
		public void DidFinishCapture(AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error) 
			=> OnFinishCapture(photoData, error);
	}
}