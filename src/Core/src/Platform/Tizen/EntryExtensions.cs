using Tizen.UIExtensions.ElmSharp;

namespace Microsoft.Maui
{
	public static class EntryExtensions
	{
		public static void UpdateText(this Entry nativeEntry, IEntry entry)
		{
			nativeEntry.Text = entry.Text ?? "";
		}

		public static void UpdateTextColor(this Entry nativeEntry, IEntry entry)
		{
			nativeEntry.TextColor = entry.TextColor.ToNative();
		}

		public static void UpdateIsPassword(this Entry nativeEntry, IEntry entry)
		{
			nativeEntry.IsPassword = entry.IsPassword;
		}
	}
}
