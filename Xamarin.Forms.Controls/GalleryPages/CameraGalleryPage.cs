using System;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Controls.GalleryPages
{
	internal class CameraGalleryPage : CarouselPage
	{
		readonly CameraView cameraView;
		Button buttonShot;

		public CameraGalleryPage()
		{
			cameraView = new CameraView
			{
				HeightRequest = 480,
				WidthRequest = 640,
				BackgroundColor = Color.Brown
			};
			cameraView.On<Windows>().SetPhotoFolder("PicturesTest");
			cameraView.On<Windows>().SetVideoFolder("VideoTest");

			var testImage = new Image
			{
				HeightRequest = 480,
				WidthRequest = 640,
				BackgroundColor = Color.Yellow
			};

			var testMediaElement = new MediaElement
			{
				HeightRequest = 480,
				WidthRequest = 640,
				BackgroundColor = Color.Aqua,
				IsVisible = false
			};

			var zoomSlider = new Slider(1, 10, 1)
			{
				IsEnabled = false
			};
			zoomSlider.ValueChanged += (_, e) => cameraView.Zoom = (float)e.NewValue;;

			var pinchGesture = new PinchGestureRecognizer();
			cameraView.GestureRecognizers.Add(pinchGesture);
			float startedZoom = cameraView.Zoom;
			pinchGesture.PinchUpdated += (_, e) => {
				if (e.Status == GestureStatus.Running)
					zoomSlider.Value = startedZoom * (float)e.Scale;
				startedZoom = cameraView.Zoom;
			};

			buttonShot = new Button
			{
				BackgroundColor = Color.FromRgba(255, 255, 255, 80),
				TextColor = Color.Black,
				Text = "Shot",
				Command = new Command(() => { 
					cameraView.Shutter();
					buttonShot.Text = cameraView.CaptureOptions != CameraCaptureOptions.Video ? "Shot" : "Stop record";
					}),
				IsEnabled = false
			};

			cameraView.MediaCaptured += (_, e) =>
			{
				switch (cameraView.CaptureOptions)
				{
					default:
					case CameraCaptureOptions.Default:
					case CameraCaptureOptions.Photo:
						testMediaElement.IsVisible = false;
						testImage.IsVisible = true;
						testImage.Source = e.Image;
						buttonShot.Text = "Shot";
						break;
					case CameraCaptureOptions.Video:
						testImage.IsVisible = false;
						testMediaElement.IsVisible = true;
						testMediaElement.Source = e.Video;
						buttonShot.Text = "Start record";
						break;
				}
			};

			var cameraControls = new Grid();
			cameraControls.AddChild(buttonShot, 0, 0);
			cameraControls.AddChild(CreatePicker<CameraOptions>("CameraPosition", c => cameraView.CameraOptions = c), 1, 0);
			cameraControls.AddChild(zoomSlider, 0, 1, 2);

			cameraView.OnAvailable += (_, available) =>
			{
				if (available)
				{
					zoomSlider.Value = cameraView.Zoom;
					var max = cameraView.MaxZoom;
					if (max > zoomSlider.Minimum && max > zoomSlider.Value)
						zoomSlider.Maximum = max;
					else
						zoomSlider.Maximum = zoomSlider.Minimum + 1; // if max == min throws exception
				}
				buttonShot.IsEnabled = available;
				zoomSlider.IsEnabled = available;
			};

			var previewLayout = new AbsoluteLayout();
			previewLayout.Children.Add(cameraView, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			previewLayout.Children.Add(cameraControls, new Rectangle(.5, 1, 1, 200), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

			Children.Add(new ContentPage
			{
				Title = "Preview",
				Content = previewLayout
			});

			Children.Add(new ContentPage
			{
				Title = "Settings / Result",
				Content = new ScrollView
				{
					Content = new StackLayout
					{
						Children = {
							new Label { Text = "Flash" },
							CreatePicker<CameraFlashMode>("Flash", f => cameraView.FlashMode = f),
							new Label { Text = "Video" },
							CreateSwitch((v) => {
								if (v)
								{
									cameraView.CaptureOptions = CameraCaptureOptions.Video;
									buttonShot.Text = "Start record";
 								} 
								else
								{
									cameraView.CaptureOptions = CameraCaptureOptions.Photo;
									buttonShot.Text = "Shot";
								}
							}),
							new Label { Text = "Video Stabilization" },
							CreateSwitch(v => cameraView.VideoStabilization = v),
							new Label { Text = "Save photo to file" },
							CreateSwitch(v => cameraView.SavePhotoToFile = v),
							new Label { Text = "PreviewAspect" },
							CreatePicker<Aspect>("PreviewAspect", a => cameraView.PreviewAspect = a),
							new Label { Text = "[Android] System sound" },
							CreateSwitch(v => cameraView.On<Android>().SetSutterSound(v), isToggled: true),
							new Label { Text = "[Android] Mirror front preview" },
							CreateSwitch(v => cameraView.On<Android>().SetMirrorFrontPreview(v)),
							testImage,
							testMediaElement
						}
					}
				}
			});
		}

		Switch CreateSwitch(Action<bool> toggled, bool isToggled = false)
		{
			var switсh = new Switch
			{
				IsToggled = isToggled
			};
			switсh.Toggled += (_, e) => toggled(e.Value);
			return switсh;
		}

		Picker CreatePicker<T>(string title, Action<T> Changed) where T : struct
		{
			var picker = new Picker
			{
				Title = title,
				BackgroundColor = Color.FromRgba(255, 255, 255, 80),
				TextColor = Color.Black
			};

			foreach (var item in Enum.GetValues(typeof(T)))
				picker.Items.Add(item.ToString());

			picker.SelectedIndex = 0;

			picker.SelectedIndexChanged += (_, __) => {
				Enum.TryParse(picker.SelectedItem.ToString(), out T en);
				Changed.Invoke(en);
			};

			return picker;
		}
	}
}
