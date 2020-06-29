using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class CameraViewUnitTests : BaseTestFixture
	{
		[Test]
		public void TestConstructor()
		{
			CameraView camera = new CameraView();

			Assert.IsFalse(camera.IsBusy);
			Assert.IsFalse(camera.IsAvailable);
			Assert.AreEqual(camera.CameraOptions, CameraOptions.Default);
			Assert.IsFalse(camera.SavePhotoToFile);
			Assert.AreEqual(camera.CaptureOptions, CameraCaptureOptions.Default);
			Assert.IsFalse(camera.KeepScreenOn);
			Assert.AreEqual(camera.FlashMode, CameraFlashMode.Off);
		}

		[Test]
		public void TestOnMediaCaptured()
		{
			CameraView camera = new CameraView();

			bool fired = false;
			var args = new MediaCapturedEventArgs();
			camera.MediaCaptured += (_, e) => fired = e == args;
			camera.RaiseMediaCaptured(args);

			Assert.IsTrue(fired);
		}

		[Test]
		public void TestOnMediaCapturedFailed()
		{
			CameraView camera = new CameraView();

			bool fired = false;
			camera.MediaCaptureFailed += (_, e) => fired = e == "123";
			camera.RaiseMediaCaptureFailed("123");

			Assert.IsTrue(fired);
		}

		[Test]
		public void TestOnShitterClicked()
		{
			CameraView camera = new CameraView();

			bool fired = false;
			camera.ShutterClicked += (sender, e) => fired = true;
			camera.Shutter();

			Assert.IsTrue(fired);
		}
	}
}
