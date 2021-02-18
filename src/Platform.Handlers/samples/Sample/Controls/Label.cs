using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class Label : View, ILabel
	{
		public Label()
		{

		}

		public string Text { get; set; }

		public Color TextColor { get; set; }
	}
}