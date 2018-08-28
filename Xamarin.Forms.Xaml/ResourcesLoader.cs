using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using System.Xml;
using System.Diagnostics;

[assembly:Dependency(typeof(Xamarin.Forms.Xaml.ResourcesLoader))]
namespace Xamarin.Forms.Xaml
{
	class ResourcesLoader : IResourcesLoader
	{
		public T CreateFromResource<T>(string resourcePath, Assembly assembly, IXmlLineInfo lineInfo) where T: new()
		{
			return new T().LoadFromXaml(GetResource(resourcePath, assembly, lineInfo));
		}

		public string GetResource(string resourcePath, Assembly assembly, IXmlLineInfo lineInfo)
		{
			var alternateResource = Xamarin.Forms.Internals.ResourceLoader.ResourceProvider?.Invoke(assembly.GetName(), resourcePath);
			if (alternateResource != null)
				return alternateResource;

			var resourceId = XamlResourceIdAttribute.GetResourceIdForPath(assembly, resourcePath);
			if (resourceId == null) // checking relative resource path
			{
				Debug.WriteLine($"Resource '{resourcePath}' not found. Checking it in 'Resources' path");
				resourceId = XamlResourceIdAttribute.GetResourceIdForPath(assembly, $"Resources/{resourcePath}");
			}
			if (resourceId == null)
				throw new XamlParseException($"Resource '{resourcePath}' not found.", lineInfo);

			using (var stream = assembly.GetManifestResourceStream(resourceId)) {
				if (stream == null)
					throw new XamlParseException($"No resource found for '{resourceId}'.", lineInfo);
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
		}
	}
}