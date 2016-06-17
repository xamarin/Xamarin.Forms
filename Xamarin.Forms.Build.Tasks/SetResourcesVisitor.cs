using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Build.Tasks
{
	class SetResourcesVisitor : IXamlNodeVisitor
	{
		public SetResourcesVisitor(ILContext context)
		{
			Context = context;
			Module = context.Body.Method.Module;
		}

		public ILContext Context { get; }

		ModuleDefinition Module { get; }

		public bool VisitChildrenFirst
		{
			get { return false; }
		}

		public bool StopOnDataTemplate
		{
			get { return true; }
		}

		public bool StopOnResourceDictionary
		{
			get { return false; }
		}

		public void Visit(ValueNode node, INode parentNode)
		{
			XmlName propertyName;
			if (!SetPropertiesVisitor.TryGetPropertyName(node, parentNode, out propertyName))
			{
				if (!IsCollectionItem(node, parentNode))
					return;
				string contentProperty;
				if (!Context.Variables.ContainsKey((IElementNode)parentNode))
					return;
				var parentVar = Context.Variables[(IElementNode)parentNode];
				if ((contentProperty = SetPropertiesVisitor.GetContentProperty(parentVar.VariableType)) != null)
					propertyName = new XmlName(((IElementNode)parentNode).NamespaceURI, contentProperty);
				else
					return;
			}

			if (node.SkipPrefix((node.NamespaceResolver ?? parentNode.NamespaceResolver)?.LookupPrefix(propertyName.NamespaceURI)))
				return;
			if (propertyName.NamespaceURI == "http://schemas.openxmlformats.org/markup-compatibility/2006" &&
				propertyName.LocalName == "Ignorable")
			{
				(parentNode.IgnorablePrefixes ?? (parentNode.IgnorablePrefixes = new List<string>())).AddRange(
					(node.Value as string).Split(','));
				return;
			}
			if (propertyName.LocalName != "MergedWith")
				return;
			SetPropertiesVisitor.SetPropertyValue(Context.Variables[(IElementNode)parentNode], propertyName, node, Context, node);
		}

		public void Visit(MarkupNode node, INode parentNode)
		{
		}

		public void Visit(ElementNode node, INode parentNode)
		{
			//Set Resources in ResourcesDictionaries
			if (IsCollectionItem(node, parentNode) && parentNode is IElementNode)
			{
				// Collection element, implicit content, or implicit collection element.
				var parentVar = Context.Variables[(IElementNode)parentNode];
				if (parentVar.VariableType.ImplementsInterface(Module.Import(typeof (IEnumerable))))
				{
					if ((parentVar.VariableType.FullName == "Xamarin.Forms.ResourceDictionary" ||
						parentVar.VariableType.Resolve().BaseType.FullName == "Xamarin.Forms.ResourceDictionary") &&
					    !node.Properties.ContainsKey(XmlName.xKey))
					{
						node.Accept(new SetPropertiesVisitor(Context), parentNode);

						if (node.XmlType.Name != "Style")
							throw new XamlParseException("resources in ResourceDictionary require a x:Key attribute", node);

						//if this node is an IMarkupExtension, invoke ProvideValue() and replace the variable
						var vardef = Context.Variables[node];
						var vardefref = new VariableDefinitionReference(vardef);
						Context.IL.Append(SetPropertiesVisitor.ProvideValue(vardefref, Context, Module, node));
						if (vardef != vardefref.VariableDefinition)
						{
							vardef = vardefref.VariableDefinition;
							Context.Body.Variables.Add(vardef);
							Context.Variables[node] = vardef;
						}

						Context.IL.Emit(OpCodes.Ldloc, parentVar);
						Context.IL.Emit(OpCodes.Ldloc, Context.Variables[node]);
						Context.IL.Emit(OpCodes.Callvirt,
							Module.Import(
								Module.Import(typeof (ResourceDictionary))
									.Resolve()
									.Methods.Single(md => md.Name == "Add" && md.Parameters.Count == 1)));
					}
					else if ((parentVar.VariableType.FullName == "Xamarin.Forms.ResourceDictionary" ||
						parentVar.VariableType.Resolve().BaseType.FullName == "Xamarin.Forms.ResourceDictionary") &&
					         node.Properties.ContainsKey(XmlName.xKey))
					{
						node.Accept(new SetPropertiesVisitor(Context), parentNode);

						//if this node is an IMarkupExtension, invoke ProvideValue() and replace the variable
						var vardef = Context.Variables[node];
						var vardefref = new VariableDefinitionReference(vardef);
						Context.IL.Append(SetPropertiesVisitor.ProvideValue(vardefref, Context, Module, node));
						if (vardef != vardefref.VariableDefinition)
						{
							vardef = vardefref.VariableDefinition;
							Context.Body.Variables.Add(vardef);
							Context.Variables[node] = vardef;
						}

						//						IL_0013:  ldloc.0 
						//						IL_0014:  ldstr "key"
						//						IL_0019:  ldstr "foo"
						//						IL_001e:  callvirt instance void class [Xamarin.Forms.Core]Xamarin.Forms.ResourceDictionary::Add(string, object)
						Context.IL.Emit(OpCodes.Ldloc, parentVar);
						Context.IL.Emit(OpCodes.Ldstr, (node.Properties[XmlName.xKey] as ValueNode).Value as string);
						var varDef = Context.Variables[node];
						Context.IL.Emit(OpCodes.Ldloc, varDef);
						if (varDef.VariableType.IsValueType)
							Context.IL.Emit(OpCodes.Box, Module.Import(varDef.VariableType));
						Context.IL.Emit(OpCodes.Callvirt,
							Module.Import(
								Module.Import(typeof (ResourceDictionary))
									.Resolve()
									.Methods.Single(md => md.Name == "Add" && md.Parameters.Count == 2)));
					}
				}
			}

			//Set ResourcesDictionaries to their parents
			XmlName propertyName;
			if (SetPropertiesVisitor.TryGetPropertyName(node, parentNode, out propertyName) &&
			    (propertyName.LocalName == "Resources" || propertyName.LocalName.EndsWith(".Resources", StringComparison.Ordinal)) &&
				(Context.Variables[node].VariableType.FullName == "Xamarin.Forms.ResourceDictionary" ||
					Context.Variables[node].VariableType.Resolve().BaseType.FullName == "Xamarin.Forms.ResourceDictionary"))
				SetPropertiesVisitor.SetPropertyValue(Context.Variables[(IElementNode)parentNode], propertyName, node, Context, node);
		}

		public void Visit(RootNode node, INode parentNode)
		{
		}

		public void Visit(ListNode node, INode parentNode)
		{
		}

		static bool IsCollectionItem(INode node, INode parentNode)
		{
			var parentList = parentNode as IListNode;
			if (parentList == null)
				return false;
			return parentList.CollectionItems.Contains(node);
		}
	}
}