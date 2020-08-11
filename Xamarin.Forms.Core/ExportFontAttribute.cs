using System;
namespace Xamarin.Forms
{
	[Internals.Preserve(AllMembers = true)]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class ExportFontAttribute : Attribute
	{
		public string Alias { get; set; }
		public string FontName { get; set; } // Required for some UWP fonts

		public ExportFontAttribute(string fontFileName)
		{
			FontFileName = fontFileName;
		}

		public string EmbeddedFontResourceId { get; set; }
		public string FontFileName { get; }
	}
}
