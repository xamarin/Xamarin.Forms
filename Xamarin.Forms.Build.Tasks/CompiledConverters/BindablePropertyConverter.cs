using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Build.Tasks;
using Xamarin.Forms.Xaml;

using static System.String;

namespace Xamarin.Forms.Core.XamlC
{
	class BindablePropertyConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ModuleDefinition module, BaseNode node)
		{
			if (IsNullOrEmpty(value)) {
				yield return Instruction.Create(OpCodes.Ldnull);
				yield break;
			}
			var bpRef = GetBindablePropertyFieldReference(value, module, node);
			yield return Instruction.Create(OpCodes.Ldsfld, bpRef);
		}

		public FieldReference GetBindablePropertyFieldReference(string value, ModuleDefinition module, BaseNode node)
		{
			FieldReference bpRef = null;
			string typeName = null, propertyName = null;

			var parts = value.Split('.');
			if (parts.Length == 1) {
				var parent = node.Parent?.Parent as IElementNode;
				if ((node.Parent as ElementNode)?.XmlType.NamespaceUri == "http://xamarin.com/schemas/2014/forms" && (node.Parent as ElementNode)?.XmlType.Name == "Setter") {
					if (parent.XmlType.NamespaceUri == "http://xamarin.com/schemas/2014/forms" &&
						(parent.XmlType.Name == "Trigger" || parent.XmlType.Name == "DataTrigger" || parent.XmlType.Name == "MultiTrigger" || parent.XmlType.Name == "Style")) {
						var ttnode = (parent as ElementNode).Properties [new XmlName("", "TargetType")];
						if (ttnode is ValueNode)
							typeName = (ttnode as ValueNode).Value as string;
						else if (ttnode is IElementNode)
							typeName = ((ttnode as IElementNode).CollectionItems.FirstOrDefault() as ValueNode)?.Value as string ?? ((ttnode as IElementNode).Properties [new XmlName("", "TypeName")] as ValueNode)?.Value as string;
					}
				} else if ((node.Parent as ElementNode)?.XmlType.NamespaceUri == "http://xamarin.com/schemas/2014/forms" && (node.Parent as ElementNode)?.XmlType.Name == "Trigger")
					typeName = ((node.Parent as ElementNode).Properties [new XmlName("", "TargetType")] as ValueNode).Value as string;
				propertyName = parts [0];
			} else if (parts.Length == 2) {
				typeName = parts [0];
				propertyName = parts [1];
			} else
				throw new XamlParseException($"Cannot convert \"{value}\" into {typeof(BindableProperty)}", node);

			var typeRef = GetTypeReference(typeName, module, node);
			if (typeRef == null)
				throw new XamlParseException($"Can't resolve {typeName}", node);
			bpRef = GetBindablePropertyFieldReference(typeRef, propertyName, module);
			if (bpRef == null)
				throw new XamlParseException($"Can't resolve {propertyName} on {typeRef.Name}", node);
			return bpRef;
		}

		public static TypeReference GetTypeReference(string xmlType, ModuleDefinition module, BaseNode iNode)
		{
			var split = xmlType.Split(':');
			if (split.Length > 2)
				throw new XamlParseException($"Type \"{xmlType}\" is invalid", iNode);

			string prefix, name;
			if (split.Length == 2) {
				prefix = split [0];
				name = split [1];
			} else {
				prefix = "";
				name = split [0];
			}
			var namespaceuri = iNode.NamespaceResolver.LookupNamespace(prefix) ?? "";
			return XmlTypeExtensions.GetTypeReference(namespaceuri, name, module, iNode);
		}

		public static FieldReference GetBindablePropertyFieldReference(TypeReference typeRef, string propertyName, ModuleDefinition module)
		{
			TypeReference declaringTypeReference;
			FieldReference bpRef = typeRef.GetField(fd => fd.Name == $"{propertyName}Property" && fd.IsStatic && fd.IsPublic, out declaringTypeReference);
			if (bpRef != null) {
				bpRef = module.Import(bpRef.ResolveGenericParameters(declaringTypeReference));
				bpRef.FieldType = module.Import(bpRef.FieldType);
			}
			return bpRef;
		}
	}
}