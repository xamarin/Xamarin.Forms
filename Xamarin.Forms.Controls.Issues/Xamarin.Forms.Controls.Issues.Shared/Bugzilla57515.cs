using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 57515, "PinchGestureRecognizer not getting called on Android ", PlatformAffected.Android)]
	public class Bugzilla57515 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Content = new PinchToZoomContainer
			{
				AutomationId = "zoomContainer",
				Content = new Image
				{
					AutomationId = "zoomImg",
					Source = ImageSource.FromFile("oasis.jpg")
				}
			};
		}

		class PinchToZoomContainer : ContentView
		{
			double currentScale = 1;
			double startScale = 1;
			double xOffset = 0;
			double yOffset = 0;

			public PinchToZoomContainer()
			{
				var pinchGesture = new PinchGestureRecognizer();
				pinchGesture.PinchUpdated += OnPinchUpdated;
				GestureRecognizers.Add(pinchGesture);
			}

			void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
			{
				if (e.Status == GestureStatus.Started)
				{
					// Store the current scale factor applied to the wrapped user interface element,
					// and zero the components for the center point of the translate transform.
					startScale = Content.Scale;
					Content.AnchorX = 0;
					Content.AnchorY = 0;
				}
				if (e.Status == GestureStatus.Running)
				{
					// Calculate the scale factor to be applied.
					currentScale += (e.Scale - 1) * startScale;
					currentScale = Math.Max(1, currentScale);

					// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
					// so get the X pixel coordinate.
					double renderedX = Content.X + xOffset;
					double deltaX = renderedX / Width;
					double deltaWidth = Width / (Content.Width * startScale);
					double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

					// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
					// so get the Y pixel coordinate.
					double renderedY = Content.Y + yOffset;
					double deltaY = renderedY / Height;
					double deltaHeight = Height / (Content.Height * startScale);
					double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

					// Calculate the transformed element pixel coordinates.
					double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
					double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

					// Apply translation based on the change in origin.
					Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
					Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

					// Apply scale factor
					Content.Scale = currentScale;
				}
				if (e.Status == GestureStatus.Completed)
				{
					// Store the translation delta's of the wrapped user interface element.
					xOffset = Content.TranslationX;
					yOffset = Content.TranslationY;
				}
			}
		}

#if UITEST
		[Test]
		public void Bugzilla57515Test()
		{
			
			var img = RunningApp.Query("zoomImg")[0];
			var rect1 = img.Rect;
			RunningApp.PinchToZoomIn(c => c.Marked("zoomContainer"));
			var img2 = RunningApp.Query("zoomImg")[0];
			var rect2 = img2.Rect;
			Assert.LessOrEqual(rect2.X, rect1.X);
		}
#endif
	}

	public static class DoubleExtensions
	{
		public static double Clamp(this double self, double min, double max)
		{
			return Math.Min(max, Math.Max(self, min));
		}
	}
}