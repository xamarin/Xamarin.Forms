namespace Xamarin.Forms.Platform.WPF
{
	public interface ICellRenderer : IRegisterable
	{
		System.Windows.DataTemplate GetTemplate(Cell cell);
	}
}