using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public class DataPackageView
	{
		internal DataPackage DataPackage { get; }
		internal DataPackagePropertySetView PropertiesInternal { get; }
		public DataPackagePropertySetView Properties { get; }

		public DataPackageView(DataPackage dataPackage)
		{
			_ = dataPackage ?? throw new System.ArgumentNullException(nameof(dataPackage));
			DataPackage = dataPackage;
			PropertiesInternal = new DataPackagePropertySetView(dataPackage.PropertiesInternal);
			Properties = new DataPackagePropertySetView(dataPackage.Properties);
		}

		public Task<ImageSource> GetImageAsync()
		{
			return Task.FromResult(DataPackage.Image);
		}

		public Task<string> GetTextAsync()
		{
			return Task.FromResult(DataPackage.Text);
		}
	}
}
