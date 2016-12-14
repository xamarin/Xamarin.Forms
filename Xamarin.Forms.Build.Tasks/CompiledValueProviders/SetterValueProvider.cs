﻿using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;
using Xamarin.Forms.Build.Tasks;

namespace Xamarin.Forms.Core.XamlC
{
	class SetterValueProvider : ICompiledValueProvider
	{
		public IEnumerable<Instruction> ProvideValue(VariableDefinitionReference vardefref, ModuleDefinition module, BaseNode node, ILContext context)
		{
			var valueNode = ((IElementNode)node).Properties[new XmlName("", "Value")];

			//if it's an elementNode, there's probably no need to convert it
			if (valueNode is IElementNode)
				yield break;

			var value = ((string)((ValueNode)valueNode).Value);
			var bpNode = ((ValueNode)((IElementNode)node).Properties[new XmlName("", "Property")]);
			var bpRef = (new BindablePropertyConverter()).GetBindablePropertyFieldReference((string)bpNode.Value, module, bpNode);

			TypeReference _;
			var setValueRef = module.Import(module.Import(typeof(Setter)).GetProperty(p => p.Name == "Value", out _).SetMethod);

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