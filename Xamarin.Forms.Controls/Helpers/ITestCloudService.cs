namespace Xamarin.Forms.Controls
{
	public interface ITestCloudService
	{
		string GetTestCloudDevice();

		string GetTestCloudDeviceName();
		bool IsOnTestCloud();
	}
}