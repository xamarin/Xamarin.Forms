using System.ComponentModel;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TextTransformUtilites
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTransformedText(string source, TextTransform textTransform)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;

			switch (textTransform)
			{
				case TextTransform.None:
				default:
					return source;
				case TextTransform.Lowercase:
					return source.ToLowerInvariant();
				case TextTransform.Uppercase:
					return source.ToUpperInvariant();
			}
		}
	}
}