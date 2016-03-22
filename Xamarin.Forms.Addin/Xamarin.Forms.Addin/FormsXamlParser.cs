using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Core;
using MonoDevelop.Ide.TypeSystem;
using MonoDevelop.Projects;
using MonoDevelop.Xml.StateEngine;
using Xamarin.Forms.Xaml;

namespace XamarinStudio.Forms
{
	public class FormsXamlParser : TypeSystemParser
	{
		public override ParsedDocument Parse(bool storeAst, string fileName, TextReader content, Project project = null)
		{
			var errors = new List<Error>();
			var parser = new Parser(new XmlFreeState(), true);

			try
			{
				parser.Parse(content);
			}
			catch (Exception ex)
			{
				LoggingService.LogError("Unhandled error parsing xaml document '" + (fileName ?? "") + "'", ex);
				errors.Add(new Error(ErrorType.Error, "Unhandled error parsing xaml document: " + ex.Message));
			}

			errors.AddRange(parser.Errors);

			var xdoc = parser.Nodes.GetRoot();
			if (xdoc?.RootElement == null)
				errors.Add(new Error(ErrorType.Error, "No root node found.", 1, 1));

			string rootNamespace = null, rootType = null;
			if (xdoc?.RootElement != null)
			{
				var rootClass = xdoc.RootElement.Attributes[new XName("x", "Class")];
				if (rootClass == null)
					errors.Add(new Error(ErrorType.Error, "Root node does not contain an x:Class attribute.", 1, 1));
				else
				{
					string rootAssembly;
					XmlnsHelper.ParseXmlns(rootClass.Value, out rootType, out rootNamespace, out rootAssembly);
				}

				if (!xdoc.RootElement.IsEnded)
					xdoc.RootElement.End(parser.Location);
			}

			var result = new FormsParsedDocument(fileName, rootType, rootNamespace, xdoc);
			result.Add(errors);

			return result;
		}
	}
}