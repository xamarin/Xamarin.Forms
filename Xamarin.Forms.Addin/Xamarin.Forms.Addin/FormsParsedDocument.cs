using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using MonoDevelop.Xml.StateEngine;
using Xamarin.Forms.Xaml;

namespace XamarinStudio.Forms
{
	public class FormsParsedDocument : XmlParsedDocument
	{
		readonly string _rootNamespace;
		readonly string _rootType;

		public FormsParsedDocument(string fileName, string rootType, string rootNamespace, XDocument xdoc) : base(fileName)
		{
			XDocument = xdoc;
			_rootType = rootType;
			_rootNamespace = rootNamespace;
		}

		public override IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions
		{
			get
			{
				IList<IUnresolvedTypeDefinition> result = base.TopLevelTypeDefinitions ?? new List<IUnresolvedTypeDefinition>();
				if (XDocument?.RootElement == null)
					return result;

				var rootRegion = XDocument.RootElement.Region;
				if (XDocument.RootElement.IsClosed)
				{
					rootRegion = new DomRegion(XDocument.RootElement.Region.FileName, XDocument.RootElement.Region.Begin,
						XDocument.RootElement.ClosingTag.Region.End);
				}

				var declType = new DefaultUnresolvedTypeDefinition(_rootNamespace, _rootType)
				{
					Kind = TypeKind.Class,
					Accessibility = Accessibility.Public,
					Region = rootRegion,
					BaseTypes = { new DefaultUnresolvedTypeDefinition(XDocument.RootElement.Name.FullName) }
				};

				var initializeComponent = new DefaultUnresolvedMethod(declType, "InitializeComponent")
				{
					ReturnType = KnownTypeReference.Void,
					Accessibility = Accessibility.Private
				};
				declType.Members.Add(initializeComponent);

				var nameAttributes = new[] { new XName("x", "Name"), new XName("Name") };
				foreach (var el in XDocument.RootElement.AllDescendentElements)
				{
					foreach (var nameAtt in nameAttributes)
					{
						var name = el.Attributes[nameAtt];
						if (name != null && name.IsComplete)
						{
							var type = ResolveType(el);
							if (type == null)
								Add(new Error(ErrorType.Error, "Could not find namespace for '" + el.Name.FullName + "'.", el.Region.Begin));
							else
							{
								declType.Members.Add(new DefaultUnresolvedField(declType, name.Value)
								{
									Accessibility = Accessibility.Private,
									Region = el.Region,
									ReturnType = type
								});
							}
						}
					}
				}

				result.Add(declType);
				return result;
			}
		}

		static ITypeReference ResolveType(XElement el)
		{
			var name = el.Name.Name;
			var ns = GetNamespace(el);

			if (ns == null)
				return null;

			return GetElementType(ns, name);
		}

		static string GetNamespace(XElement el)
		{
			XName attName;
			if (el.Name.HasPrefix)
				attName = new XName("xmlns", el.Name.Prefix);
			else
			{
				attName = new XName("xmlns");
				var att = el.Attributes[attName];
				if (att != null)
					return att.Value;
			}

			foreach (var node in el.Parents)
			{
				var parentElement = node as XElement;
				var att = parentElement?.Attributes[attName];
				if (att != null)
					return att.Value;
			}
			return null;
		}

		static ITypeReference GetElementType(string namespaceUri, string elementName)
		{
			string ns;

			if (!XmlnsHelper.IsCustom(namespaceUri))
				ns = "Xamarin.Forms";
			else
			{
				string typename;
				string asmstring;

				XmlnsHelper.ParseXmlns(namespaceUri, out typename, out ns, out asmstring);
			}

			if (elementName.Contains(":"))
				elementName = elementName.Substring(elementName.LastIndexOf(':') + 1);

			return new GetClassTypeReference(ns, elementName);
		}
	}
}