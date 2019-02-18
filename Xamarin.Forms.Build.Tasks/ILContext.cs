using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Build.Tasks
{
	class ILContext
	{
		public ILContext(ILProcessor il, MethodBody body, ModuleDefinition module, FieldDefinition parentContextValues = null, Func<string, (TypeReference, IEnumerable<Instruction>)> parentContextNamedFinder = null)
		{
			IL = il;
			Body = body;
			Values = new Dictionary<IValueNode, object>();
			Variables = new Dictionary<IElementNode, VariableDefinition>();
			Scopes = new Dictionary<INode, (VariableDefinition, Dictionary<string, VariableDefinition>)>();
			TypeExtensions = new Dictionary<INode, TypeReference>();
			ParentContextValues = parentContextValues;
			ParentContextNamedFinder = parentContextNamedFinder;
			Module = module;
		}

		public Dictionary<IValueNode, object> Values { get; private set; }

		public Dictionary<IElementNode, VariableDefinition> Variables { get; private set; }

		public Dictionary<INode, (VariableDefinition, Dictionary<string, VariableDefinition>)> Scopes { get; private set; }

		public Dictionary<INode, TypeReference> TypeExtensions { get; }

		public FieldDefinition ParentContextValues { get; private set; }

		public Func<string, (TypeReference, IEnumerable<Instruction>)> ParentContextNamedFinder;

		public object Root { get; set; } //FieldDefinition or VariableDefinition

		public ILProcessor IL { get; private set; }

		public MethodBody Body { get; private set; }

		public ModuleDefinition Module { get; private set; }

		public VariableDefinition FindByName(string name, INode node)
		{
			for (var cursor = node; cursor != null; cursor = cursor.Parent)
				if (Scopes.TryGetValue(node, out var scope) && scope.Item2.TryGetValue(name, out var variable))
					return variable;

			return null;
		}
	}
}