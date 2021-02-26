using Android.Widget;

namespace Xamarin.Platform
{
	public static class EditorExtensions
	{
		public static void UpdateText(this EditText editText, IEditor editor)
		{
			editText.Text = editor.Text;
		}

		public static void UpdateTextColor(this EditText editText, IEditor editor)
		{
			editText.UpdateTextColor(editor, null);
		}

		public static void UpdateTextColor(this EditText editText, IEditor editor, TextColorSwitcher? textColorSwitcher)
		{
			textColorSwitcher ??= new TextColorSwitcher(editText.TextColors);
			textColorSwitcher.UpdateTextColor(editText, editor.TextColor);
		}
	}
}
