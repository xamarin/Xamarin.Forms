using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class Editor : Xamarin.Forms.View, IEditor
	{
		public string Text { get; set; }

		public Color TextColor { get; set; } = Color.Default;

		public new double Width
		{
			get { return WidthRequest; }
			set { WidthRequest = value; }
		}

		public new double Height
		{
			get { return HeightRequest; }
			set { HeightRequest = value; }
		}
	}
}