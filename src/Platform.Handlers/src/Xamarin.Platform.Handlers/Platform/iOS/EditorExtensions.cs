using UIKit;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public static class EditorExtensions
	{
		public static void UpdateText(this UITextView textView, IEditor editor)
		{
			textView.Text = editor.Text;
		}

		public static void UpdateTextColor(this UITextView textView, IEditor editor)
		{
			textView.UpdateTextColor(editor, null);
		}

		public static void UpdateTextColor(this UITextView textView, IEditor editor, UIColor? defaultTextColor)
		{
			if (editor.TextColor == Color.Default)
			{
				if (defaultTextColor != null)
					textView.TextColor = defaultTextColor;
			}
			else
				textView.TextColor = editor.TextColor.ToNative();
		}

	}
}
