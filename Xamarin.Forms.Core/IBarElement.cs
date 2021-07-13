namespace Xamarin.Forms
{
	interface IBarElement
	{
		Color BarBackgroundColor { get; }
		Brush BarBackground { get; }
		Color BarTextColor { get; }
		string BarFontFamily { get; }
		double BarFontSize { get; }
		FontAttributes BarFontAttributes { get; }
		Font Font { get; }
	}
}