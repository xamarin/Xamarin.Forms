using ELabel = ElmSharp.Label;

namespace Xamarin.Platform
{
	public static class LabelExtensions
	{
		public static void UpdateText(this ELabel nativeLabel, ILabel label)
		{
			nativeLabel.Text = label.Text ?? "";
		}
	}
}
