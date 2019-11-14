using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class EmbeddedResourceLoader
	{
		static Assembly ExecutingAssembly;

		static void EnsureAssemblyIsDefined()
		{
			if(ExecutingAssembly == null)
			{
				throw new Exception($"Before using {nameof(EmbeddedResourceLoader)} you need to call {nameof(SetExecutingAssembly)}");
			}
		}

		public static void SetExecutingAssembly(Assembly assembly)
		{
			ExecutingAssembly = assembly;
		}

		public static byte[] GetEmbeddedResourceBytes(string resourceFileName)
		{
			EnsureAssemblyIsDefined();
			return GetEmbeddedResourceBytes(resourceFileName, ExecutingAssembly);
		}

		public static byte[] GetEmbeddedResourceBytes(string resourceFileName, Assembly assembly)
		{
			Stream stream = GetEmbeddedResourceStream(resourceFileName, assembly);

			if(stream == null)
			{
				return null;
			}

			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}

		public static string GetEmbeddedResourcePath(string resourceFileName)
		{
			EnsureAssemblyIsDefined();
			return GetEmbeddedResourcePath(resourceFileName, ExecutingAssembly);
		}

		public static string GetEmbeddedResourcePath(string resourceFileName, Assembly assembly)
		{
			resourceFileName = CheckResourceFileName(resourceFileName);
			string[] resourceNames = assembly.GetManifestResourceNames();

			foreach (var item in resourceNames)
			{
				if (item.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase))
				{
					return item;
				}
			}

			Debug.WriteLine($"Resource {resourceFileName} not found in {assembly.FullName}.");
			return null;
		}

		public static Stream GetEmbeddedResourceStream(string resourceFileName)
		{
			EnsureAssemblyIsDefined();
			return GetEmbeddedResourceStream(resourceFileName, ExecutingAssembly);
		}

		public static Stream GetEmbeddedResourceStream(string resourceFileName, Assembly assembly)
		{
			resourceFileName = CheckResourceFileName(resourceFileName);
			string[] resourceNames = assembly.GetManifestResourceNames();

			foreach (var item in resourceNames)
			{
				if (item.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase))
				{
					return assembly.GetManifestResourceStream(item);
				}
			}

			Debug.WriteLine($"Resource {resourceFileName} not found in {assembly.FullName}.");
			return null;
		}

		public static string GetEmbeddedResourceString(string resourceFileName)
		{
			EnsureAssemblyIsDefined();
			return GetEmbeddedResourceString(resourceFileName, ExecutingAssembly);
		}

		public static string GetEmbeddedResourceString(string resourceFileName, Assembly assembly)
		{
			Stream stream = GetEmbeddedResourceStream(resourceFileName, assembly);
			
			if (stream == null)
			{
				return null;
			}

			using (var streamReader = new StreamReader(stream))
			{
				return streamReader.ReadToEnd();
			}
		}

		public static ImageSource GetImageSource(string name)
		{
			EnsureAssemblyIsDefined();
			return ImageSource.FromResource(GetEmbeddedResourcePath(name, ExecutingAssembly), ExecutingAssembly);
		}

		public static ImageSource GetImageSource(string name, Assembly assembly)
		{
			return ImageSource.FromResource(GetEmbeddedResourcePath(name, assembly), assembly);
		}

		static string CheckResourceFileName(string name)
		{
			if (name.StartsWith("."))
			{
				return name;
			}

			return "." + name;
		}
	}
}