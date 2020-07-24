namespace Xamarin.Forms
{
	public class DataPackage
	{
		public DataPackage()
		{
			Properties = new DataPackagePropertySet();
			PropertiesInternal = new DataPackagePropertySet();
			View = new DataPackageView(this);
		}

		public DataPackagePropertySet Properties { get; }
		internal DataPackagePropertySet PropertiesInternal { get; }

		public ImageSource Image { get; set; }
		public string Text { get; set; }
		public DataPackageView View { get; }
	}
}
