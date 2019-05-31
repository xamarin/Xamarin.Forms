//
// XamlLoader.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Mobile Inception
// Copyright (c) 2013-2014 Xamarin, Inc
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Xaml.Internals
{
	[Obsolete ("Replaced by ResourceLoader")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class XamlLoader
	{
		static Func<Type, string> xamlFileProvider;

		public static Func<Type, string> XamlFileProvider {
			get { return xamlFileProvider; }
			internal set {
				xamlFileProvider = value;
				Xamarin.Forms.DesignMode.IsDesignModeEnabled = true;
				//¯\_(ツ)_/¯ the previewer forgot to set that bool
				DoNotThrowOnExceptions = value != null;
			}
		}

		internal static bool DoNotThrowOnExceptions { get; set; }
	}
}

namespace Xamarin.Forms.Xaml
{
	static class XamlLoader
	{
		public static void Load(object view, Type callingType)
		{
			var xaml = GetXamlForType(callingType, view, out var useDesignProperties);
			if (string.IsNullOrEmpty(xaml))
				throw new XamlParseException(string.Format("No embeddedresource found for {0}", callingType), new XmlLineInfo());
			Load(view, xaml, useDesignProperties);
		}

		public static void Load(object view, string xaml) => Load(view, xaml, false);

		public static void Load(object view, string xaml, bool useDesignProperties)
		{
			using (var textReader = new StringReader(xaml))
			using (var reader = XmlReader.Create(textReader)) {
				while (reader.Read()) {
					//Skip until element
					if (reader.NodeType == XmlNodeType.Whitespace)
						continue;
					if (reader.NodeType == XmlNodeType.XmlDeclaration)
						continue;
					if (reader.NodeType != XmlNodeType.Element) {
						Debug.WriteLine("Unhandled node {0} {1} {2}", reader.NodeType, reader.Name, reader.Value);
						continue;
					}

					var rootnode = new RuntimeRootNode(new XmlType(reader.NamespaceURI, reader.Name, null), view, (IXmlNamespaceResolver)reader);
					XamlParser.ParseXaml(rootnode, reader);
#pragma warning disable 0618
					var doNotThrow = ResourceLoader.ExceptionHandler2 != null || Internals.XamlLoader.DoNotThrowOnExceptions;
#pragma warning restore 0618
					void ehandler(Exception e) => ResourceLoader.ExceptionHandler2?.Invoke((e, XamlFilePathAttribute.GetFilePathForObject(view)));
					Visit(rootnode, new HydrationContext {
						RootElement = view,

						ExceptionHandler = doNotThrow ? ehandler : (Action<Exception>)null
					}, useDesignProperties);
					break;
				}
			}
		}

		public static object Create(string xaml, bool doNotThrow = false) => Create(xaml, doNotThrow, false);

		public static object Create(string xaml, bool doNotThrow, bool useDesignProperties)
		{
			doNotThrow = doNotThrow || ResourceLoader.ExceptionHandler2 != null;
			void ehandler(Exception e) => ResourceLoader.ExceptionHandler2?.Invoke((e, null));

			object inflatedView = null;
			using (var textreader = new StringReader(xaml))
			using (var reader = XmlReader.Create(textreader)) {
				while (reader.Read()) {
					//Skip until element
					if (reader.NodeType == XmlNodeType.Whitespace)
						continue;
					if (reader.NodeType == XmlNodeType.XmlDeclaration)
						continue;
					if (reader.NodeType != XmlNodeType.Element) {
						Debug.WriteLine("Unhandled node {0} {1} {2}", reader.NodeType, reader.Name, reader.Value);
						continue;
					}

					var typeArguments = XamlParser.GetTypeArguments(reader);
					var rootnode = new RuntimeRootNode(new XmlType(reader.NamespaceURI, reader.Name, typeArguments), null, (IXmlNamespaceResolver)reader);
					XamlParser.ParseXaml(rootnode, reader);
					var visitorContext = new HydrationContext {
						ExceptionHandler = doNotThrow ? ehandler : (Action<Exception>)null,
					};
					var cvv = new CreateValuesVisitor(visitorContext);
					cvv.Visit((ElementNode)rootnode, null);
					inflatedView = rootnode.Root = visitorContext.Values[rootnode];
					visitorContext.RootElement = inflatedView as BindableObject;

					Visit(rootnode, visitorContext, useDesignProperties);
					break;
				}
			}
			return inflatedView;
		}

		public static IResourceDictionary LoadResources(string xaml, IResourcesProvider rootView)
		{
			void ehandler(Exception e) => ResourceLoader.ExceptionHandler2?.Invoke((e, XamlFilePathAttribute.GetFilePathForObject(rootView)));

			using (var textReader = new StringReader(xaml))
			using (var reader = XmlReader.Create(textReader)) {
				while (reader.Read()) {
					//Skip until element
					if (reader.NodeType == XmlNodeType.Whitespace)
						continue;
					if (reader.NodeType == XmlNodeType.XmlDeclaration)
						continue;
					if (reader.NodeType != XmlNodeType.Element)
					{
						Debug.WriteLine("Unhandled node {0} {1} {2}", reader.NodeType, reader.Name, reader.Value);
						continue;
					}

					//the root is set to null, and not to rootView, on purpose as we don't want to erase the current Resources of the view
					RootNode rootNode = new RuntimeRootNode(new XmlType(reader.NamespaceURI, reader.Name, null), null, (IXmlNamespaceResolver)reader);
					XamlParser.ParseXaml(rootNode, reader);
					var rNode = (IElementNode)rootNode;
					if (!rNode.Properties.TryGetValue(new XmlName("http://xamarin.com/schemas/2014/forms", "Resources"), out var resources))
						return null;

					var visitorContext = new HydrationContext
					{
						ExceptionHandler = ResourceLoader.ExceptionHandler2 != null ? ehandler : (Action<Exception>)null,
					};
					var cvv = new CreateValuesVisitor(visitorContext);
					cvv.Visit((ElementNode)resources, null);
					visitorContext.RootElement = visitorContext.Values[resources];

					resources.Accept(new XamlNodeVisitor((node, parent) => node.Parent = parent), null); //set parents for {StaticResource}
					resources.Accept(new ExpandMarkupsVisitor(visitorContext), null);
					resources.Accept(new PruneIgnoredNodesVisitor(false), null);
					resources.Accept(new NamescopingVisitor(visitorContext), null); //set namescopes for {x:Reference}
					resources.Accept(new CreateValuesVisitor(visitorContext), null);
					resources.Accept(new RegisterXNamesVisitor(visitorContext), null);
					resources.Accept(new FillResourceDictionariesVisitor(visitorContext), null);
					resources.Accept(new ApplyPropertiesVisitor(visitorContext, true), null);

					return visitorContext.Values[resources] as IResourceDictionary;
				}
			}
			return null;
		}

		static void Visit(RootNode rootnode, HydrationContext visitorContext, bool useDesignProperties)
		{
			rootnode.Accept(new XamlNodeVisitor((node, parent) => node.Parent = parent), null); //set parents for {StaticResource}
			rootnode.Accept(new ExpandMarkupsVisitor(visitorContext), null);
			rootnode.Accept(new PruneIgnoredNodesVisitor(useDesignProperties), null);
			if (useDesignProperties)
				rootnode.Accept(new RemoveDuplicateDesignNodes(), null);
			rootnode.Accept(new NamescopingVisitor(visitorContext), null); //set namescopes for {x:Reference}
			rootnode.Accept(new CreateValuesVisitor(visitorContext), null);
			rootnode.Accept(new RegisterXNamesVisitor(visitorContext), null);
			rootnode.Accept(new FillResourceDictionariesVisitor(visitorContext), null);
			rootnode.Accept(new ApplyPropertiesVisitor(visitorContext, true), null);
		}

		static string GetXamlForType(Type type, object instance, out bool useDesignProperties)
		{
			useDesignProperties = false;
			//the Previewer might want to provide it's own xaml for this... let them do that
			//the check at the end is preferred (using ResourceLoader). keep this until all the previewers are updated

			string xaml;
#pragma warning disable 0618
			if (ResourceLoader.ResourceProvider2 == null && (xaml = Internals.XamlLoader.XamlFileProvider?.Invoke(type)) != null)
				return xaml;
#pragma warning restore 0618

			var assembly = type.GetTypeInfo().Assembly;
			var resourceId = XamlResourceIdAttribute.GetResourceIdForType(type);

			var rlr = ResourceLoader.ResourceProvider2?.Invoke(new ResourceLoader.ResourceLoadingQuery {
				AssemblyName = assembly.GetName(),
				ResourcePath = XamlResourceIdAttribute.GetPathForType(type),
				Instance = instance,
			});
			var alternateXaml = rlr?.ResourceContent;

			if (alternateXaml != null) {
				useDesignProperties = rlr.UseDesignProperties;
				return alternateXaml;
			}

			if (resourceId == null)
				return LegacyGetXamlForType(type);

			using (var stream = assembly.GetManifestResourceStream(resourceId)) {
				if (stream != null)
					using (var reader = new StreamReader(stream))
						xaml = reader.ReadToEnd();
				else
					xaml = null;
			}

			return xaml;
		}

		//if the assembly was generated using a version of XamlG that doesn't outputs XamlResourceIdAttributes, we still need to find the resource, and load it
		static readonly Dictionary<Type, string> XamlResources = new Dictionary<Type, string>();
		static string LegacyGetXamlForType(Type type)
		{
			var assembly = type.GetTypeInfo().Assembly;

			string resourceId;
			if (XamlResources.TryGetValue(type, out resourceId)) {
				var result = ReadResourceAsXaml(type, assembly, resourceId);
				if (result != null)
					return result;
			}

			var likelyResourceName = type.Name + ".xaml";
			var resourceNames = assembly.GetManifestResourceNames();
			string resourceName = null;

			// first pass, pray to find it because the user named it correctly

			foreach (var resource in resourceNames) {
				if (ResourceMatchesFilename(assembly, resource, likelyResourceName)) {
					resourceName = resource;
					var xaml = ReadResourceAsXaml(type, assembly, resource);
					if (xaml != null)
						return xaml;
				}
			}

			// okay maybe they at least named it .xaml

			foreach (var resource in resourceNames) {
				if (!resource.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
					continue;

				resourceName = resource;
				var xaml = ReadResourceAsXaml(type, assembly, resource);
				if (xaml != null)
					return xaml;
			}

			foreach (var resource in resourceNames) {
				if (resource.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
					continue;

				resourceName = resource;
				var xaml = ReadResourceAsXaml(type, assembly, resource, true);
				if (xaml != null)
					return xaml;
			}

			return null;
		}

		//legacy...
		static bool ResourceMatchesFilename(Assembly assembly, string resource, string filename)
		{
			try {
				var info = assembly.GetManifestResourceInfo(resource);

				if (!string.IsNullOrEmpty(info.FileName) &&
					string.Compare(info.FileName, filename, StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}
			catch (PlatformNotSupportedException) {
				// Because Win10 + .NET Native
			}

			if (resource.EndsWith("." + filename, StringComparison.OrdinalIgnoreCase) ||
				string.Compare(resource, filename, StringComparison.OrdinalIgnoreCase) == 0)
				return true;

			return false;
		}

		//part of the legacy as well...
		static string ReadResourceAsXaml(Type type, Assembly assembly, string likelyTargetName, bool validate = false)
		{
			using (var stream = assembly.GetManifestResourceStream(likelyTargetName))
			using (var reader = new StreamReader(stream)) {
				if (validate) {
					// terrible validation of XML. Unfortunately it will probably work most of the time since comments
					// also start with a <. We can't bring in any real deps.

					var firstNonWhitespace = (char)reader.Read();
					while (char.IsWhiteSpace(firstNonWhitespace))
						firstNonWhitespace = (char)reader.Read();

					if (firstNonWhitespace != '<')
						return null;

					stream.Seek(0, SeekOrigin.Begin);
				}

				var xaml = reader.ReadToEnd();

				var pattern = String.Format("x:Class *= *\"{0}\"", type.FullName);
				var regex = new Regex(pattern, RegexOptions.ECMAScript);
				if (regex.IsMatch(xaml) || xaml.Contains(String.Format("x:Class=\"{0}\"", type.FullName)))
					return xaml;
			}
			return null;
		}

		public class RuntimeRootNode : RootNode
		{
			public RuntimeRootNode(XmlType xmlType, object root, IXmlNamespaceResolver resolver) : base(xmlType, resolver)
			{
				Root = root;
			}

			public object Root { get; internal set; }
		}

		public struct FallbackTypeInfo
		{
			public string ClrNamespace { get; internal set; }
			public string TypeName { get; internal set; }
			public string AssemblyName { get; internal set; }
			public string XmlNamespace { get; internal set; }
		}

		public struct CallbackTypeInfo
		{
			public string XmlNamespace { get; internal set; }
			public string XmlTypeName { get; internal set; }

		}

		internal static Func<IList<FallbackTypeInfo>, Type, Type> FallbackTypeResolver { get; set; }
		internal static Action<CallbackTypeInfo, object> ValueCreatedCallback  { get; set; }
		internal static Func<CallbackTypeInfo, Type, Exception, object> InstantiationFailedCallback { get; set; }
	}
}