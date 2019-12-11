using System.Linq;
using System.Xml;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;
using Xamarin.Forms.Exceptions;

using static Mono.Cecil.Cil.Instruction;
using static Mono.Cecil.Cil.OpCodes;

namespace Xamarin.Forms.Build.Tasks
{
	class DataTemplateExtension : ICompiledMarkupExtension
	{
		public IEnumerable<Instruction> ProvideValue(IElementNode node, ModuleDefinition module, ILContext context, out TypeReference typeRef)
		{
			typeRef = module.ImportReference(("Xamarin.Forms.Core", "Xamarin.Forms", "DataTemplate"));
			var name = new XmlName("", "TypeName");

			if (!node.Properties.TryGetValue(name, out INode typeNameNode) && node.CollectionItems.Any())
				typeNameNode = node.CollectionItems[0];

			if (!(typeNameNode is ValueNode valueNode))
				throw new XFException(XFException.Ecode.TypeName, node as XmlLineInfo);

			var contentTypeRef = module.ImportReference(XmlTypeExtensions.GetTypeReference(valueNode.Value as string, module, node as BaseNode))
				?? throw new XFException(XFException.Ecode.ResolveType, node as IXmlLineInfo, valueNode.Value.ToString());

			var dataTemplateCtor = module.ImportCtorReference(typeRef, new[] { module.ImportReference(("mscorlib", "System", "Type")) });
			return new List<Instruction> {
				Create(Ldtoken, module.ImportReference(contentTypeRef)),
				Create(Call, module.ImportMethodReference(("mscorlib", "System", "Type"), methodName: "GetTypeFromHandle", parameterTypes: new[] { ("mscorlib", "System", "RuntimeTypeHandle") }, isStatic: true)),
				Create(Newobj, dataTemplateCtor),
			};
		}
	}
}
