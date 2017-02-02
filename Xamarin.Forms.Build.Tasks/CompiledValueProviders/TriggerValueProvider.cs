using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;
using Xamarin.Forms.Build.Tasks;
using System.Xml;

namespace Xamarin.Forms.Core.XamlC
{
	class TriggerValueProvider : ICompiledValueProvider
	{
		public IEnumerable<Instruction> ProvideValue(VariableDefinitionReference vardefref, ModuleDefinition module, BaseNode node, ILContext context)
		{
			INode valueNode = null;
			if (!((IElementNode)node).Properties.TryGetValue(new XmlName("", "Value"), out valueNode) && ((IElementNode)node).CollectionItems.Count == 1)
				valueNode = ((IElementNode)node).CollectionItems[0];

			if (valueNode == null)
				throw new XamlParseException("Missing Value for Trigger", (IXmlLineInfo)node);

			//if it's an elementNode, there's probably no need to convert it
			if (valueNode is IElementNode)
				yield break;

			var value = ((string)((ValueNode)valueNode).Value);
			var bpNode = ((ValueNode)((IElementNode)node).Properties[new XmlName("", "Property")]);
			var bpRef = (new BindablePropertyConverter()).GetBindablePropertyFieldReference((string)bpNode.Value, module, bpNode);

			TypeReference _;
			var setValueRef = module.ImportReference(module.ImportReference(typeof(Trigger)).GetProperty(p => p.Name == "Value", out _).SetMethod);

			//push the setter
			yield return Instruction.Create(OpCodes.Ldloc, vardefref.VariableDefinition);

			//push the value
			foreach (var instruction in ((ValueNode)valueNode).PushConvertedValue(context, bpRef, valueNode.PushServiceProvider(context, bpRef: bpRef), boxValueTypes: true, unboxValueTypes: false))
				yield return instruction;

			//set the value
			yield return Instruction.Create(OpCodes.Callvirt, setValueRef);
		}
	}
}