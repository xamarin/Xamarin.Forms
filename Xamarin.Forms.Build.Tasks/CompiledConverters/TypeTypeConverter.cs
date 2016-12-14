using System;
using System.Collections.Generic;
using System.Xml;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Build.Tasks;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Core.XamlC
{
	class TypeTypeConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ModuleDefinition module, BaseNode node)
		{
			if (string.IsNullOrEmpty(value))
				goto error;

			var split = value.Split(':');
			if (split.Length > 2)
				goto error;

			XmlType xmlType;
			if (split.Length == 2)
				xmlType = new XmlType(node.NamespaceResolver.LookupNamespace(split[0]), split[1], null);
			else
				xmlType = new XmlType(node.NamespaceResolver.LookupNamespace(""), split[0], null);

			var typeRef = xmlType.GetTypeReference(module, (IXmlLineInfo)node);
			if (typeRef == null)
				goto error;

			var getTypeFromHandle = module.Import(typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) }));
			yield return Instruction.Create(OpCodes.Ldtoken, module.Import(typeRef));
			yield return Instruction.Create(OpCodes.Call, module.Import(getTypeFromHandle));
			yield break;

		error:
			throw new XamlParseException($"Cannot convert \"{value}\" into {typeof(Type)}", node);
		}
	}

}