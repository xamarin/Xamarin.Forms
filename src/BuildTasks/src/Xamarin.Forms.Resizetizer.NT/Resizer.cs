//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace Resizetizer
{
	internal class Resizer
	{
		public Resizer(SharedImageInfo info, string intermediateOutputPath, ILogger logger)
		{
			Info = info;
			Logger = logger;
			IntermediateOutputPath = intermediateOutputPath;
		}

		public ILogger Logger { get; private set; }
		public string IntermediateOutputPath { get; private set; }

		public SharedImageInfo Info { get; private set; }

		SkiaSharpTools tools;

		public string GetFileDestination(DpiPath dpi)
			=> GetFileDestination(Info, dpi, IntermediateOutputPath);

		public static string GetFileDestination(SharedImageInfo info, DpiPath dpi, string intermediateOutputPath)
		{
			var name = Path.GetFileNameWithoutExtension(info.Filename);
			var ext = Path.GetExtension(info.Filename);

			var fullIntermediateOutputPath = new DirectoryInfo(intermediateOutputPath);

			var destination = Path.Combine(fullIntermediateOutputPath.FullName, dpi.Path, name + dpi.FileSuffix + ext);

			var fileInfo = new FileInfo(destination);
			if (!fileInfo.Directory.Exists)
				fileInfo.Directory.Create();

			return destination;
		}

		public static ResizedImageInfo CopyFile(SharedImageInfo info, DpiPath dpi, string intermediateOutputPath, string inputsFile, ILogger logger, bool isAndroid = false)
		{
			var destination = Resizer.GetFileDestination(info, dpi, intermediateOutputPath);
			var androidVector = false;

			if (isAndroid && info.IsVector && !info.Resize)
			{
				// Update destination to be .xml file
				destination = Path.ChangeExtension(info.Filename, ".xml");
				androidVector = true;
			}

			if (IsUpToDate(info.Filename, destination, inputsFile, logger))
				return new ResizedImageInfo { Filename = destination, Dpi = dpi };
			
			if (androidVector)
			{
				logger.Log("Converting SVG to Android Drawable Vector: " + info.Filename);
				// Transform into an android vector drawable
				var convertErr = Svg2VectorDrawable.Svg2Vector.Convert(info.Filename, destination);
				if (!string.IsNullOrEmpty(convertErr))
					throw new Svg2AndroidDrawableConversionException(convertErr, info.Filename);
			}
			else
			{
				// Otherwise just copy it straight
				File.Copy(info.Filename, destination, true);
			}

			return new ResizedImageInfo { Filename = destination, Dpi = dpi };
		}

		static bool IsUpToDate(string inputFile, string outputFile, string inputsFile, ILogger logger)
		{
			var fileIn = new FileInfo(inputFile);
			var fileOut = new FileInfo(outputFile);
			var fileInputs = new FileInfo(inputsFile);

			if (fileIn.Exists && fileOut.Exists && fileInputs.Exists
				&& fileIn.LastWriteTimeUtc <= fileOut.LastWriteTimeUtc
				&& fileInputs.LastWriteTimeUtc <= fileOut.LastWriteTimeUtc)
			{
				logger.Log($"Skipping '{inputFile}' as output '{outputFile}' is already up to date.");
				return true;
			}

			return false;
		}

		public ResizedImageInfo Resize(DpiPath dpi, string inputsFile)
		{
			var destination = GetFileDestination(dpi);

			if (Info.IsVector)
				destination = Path.ChangeExtension(destination, ".png");

			if (IsUpToDate(Info.Filename, destination, inputsFile, Logger))
				return new ResizedImageInfo { Filename = destination, Dpi = dpi };

			if (tools == null)
			{
				tools = SkiaSharpTools.Create(Info.IsVector, Info.Filename, Info.BaseSize, Info.TintColor, Logger);
			}

			tools.Resize(dpi, destination);

			return new ResizedImageInfo { Filename = destination, Dpi = dpi };
		}
	}

	internal class ResizedImageInfo
	{
		public string Filename { get; set; }
		public DpiPath Dpi { get; set; }
	}
}
