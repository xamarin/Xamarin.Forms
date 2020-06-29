using System;
using System.IO;
using UIKit;
using Foundation;
using Photos;

namespace Xamarin.Forms.Platform.iOS
{
	public class CameraViewRenderer : ViewRenderer<CameraView, FormsCameraView>
	{
		public CameraViewRenderer()
		{
			Xamarin.Forms.CameraView.VerifyCameraViewFlagEnabled(nameof(CameraViewRenderer));
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				SetNativeControl(new FormsCameraView());
				Control.Busy += OnBusy;
				Control.Available += OnAvailability;
				Control.FinishCapture += FinishCapture;
			}

			if (e.OldElement != null)
			{
				e.OldElement.ShutterClicked -= HandleShutter;
			}

			if (e.NewElement != null)
			{
				e.NewElement.ShutterClicked += HandleShutter;
			}
		}

		void OnBusy(object sender, bool busy) => Element.IsBusy = busy;

		void OnAvailability(object sender, bool available)
		{
			Element.MaxZoom = Control.MaxZoom;
			Element.IsAvailable = available;
		}

		void RaiseImageCapture(NSData photoData)
		{
			var data = UIImage.LoadFromData(photoData).AsJPEG().ToArray();
			Device.BeginInvokeOnMainThread(() =>
			{
				Element.RaiseMediaCaptured(new MediaCapturedEventArgs()
				{
					Data = data,
					Image = ImageSource.FromStream(() => new MemoryStream(data))
				});
			});
		}

		void FinishCapture(object sender, Tuple<NSObject, NSError> e)
		{
			if (Element == null || Control == null)
				return;

			if (e.Item2 != null)
			{
				Element.RaiseMediaCaptureFailed(e.Item2.LocalizedDescription);
				return;
			}

			var photoData = e.Item1 as NSData;
			if (!Element.SavePhotoToFile && photoData != null)
			{
				RaiseImageCapture(photoData);
				return;
			}

			PHPhotoLibrary.RequestAuthorization(status =>
			{
				if (status != PHAuthorizationStatus.Authorized)
					return;

				// Save the captured file to the photo library.
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
				{
					var creationRequest = PHAssetCreationRequest.CreationRequestForAsset();
					if (photoData != null)
					{
						creationRequest.AddResource(PHAssetResourceType.Photo, photoData, null);
						RaiseImageCapture(photoData);
					}
					else if (e.Item1 is NSUrl outputFileUrl)
					{
						creationRequest.AddResource(
							PHAssetResourceType.Video,
							outputFileUrl,
							new PHAssetResourceCreationOptions
							{
								ShouldMoveFile = true
							});
						Device.BeginInvokeOnMainThread(() =>
						{
							Element.RaiseMediaCaptured(new MediaCapturedEventArgs
							{
								Video = MediaSource.FromFile(outputFileUrl.Path)
							});
						});
					}
				}, (success2, error2) =>
				{
					if (!success2)
						Console.WriteLine($"Could not save movie to photo library: {error2}");
				});
			});
		}

		protected override void Dispose(bool disposing)
		{
			Control?.Dispose();
			base.Dispose(disposing);
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Element == null || Control == null)
				return;

			switch (e.PropertyName)
			{
				case nameof(CameraView.CameraOptions):
					Control.RetrieveCameraDevice(Element.CameraOptions);
					break;
				case nameof(VisualElement.HeightProperty):
				case nameof(VisualElement.WidthProperty):
					Control.SetBounds(Element.Width, Element.Height);
					break;
				case nameof(CameraView.VideoStabilization):
					Control.VideoStabilization = Element.VideoStabilization;
					break;
				case nameof(CameraView.FlashMode):
					Control.SwitchFlash(Element.FlashMode);
					break;
				case nameof(CameraView.Zoom):
					Control.Zoom = Element.Zoom;
					break;
			}
		}

		async void HandleShutter(object sender, EventArgs e)
		{
			switch (Element.CaptureOptions)
			{
				case CameraCaptureOptions.Default:
				case CameraCaptureOptions.Photo:
					await Control?.TakePhoto();
					break;
				case CameraCaptureOptions.Video:
					if (Control == null)
						return;

					if (!Control.VideoRecorded)
						Control.StartRecord();
					else
						Control.StopRecord();
					break;
			}
		}
	}
}