using System;
using System.Drawing;
using System.IO;

namespace Resizetizer
{
	internal class SharedImageInfo
	{
		public string Filename { get; set; }

		public Size? BaseSize { get; set; }

		public bool Resize { get; set; } = true;

		public Color? TintColor { get; set; }

		public bool IsVector => IsVectorFilename(Filename);

		public bool IsAppIcon { get; set; }

		public string ForegroundFilename { get; set; }

		public bool ForegroundIsVector => IsVectorFilename(ForegroundFilename);

		public double ForegroundScale { get; set; } = 1.0;

		private static bool IsVectorFilename(string filename)
			=> Path.GetExtension(filename)?.Equals(".svg", StringComparison.OrdinalIgnoreCase) ?? false;
	}
}
