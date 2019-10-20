using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Xamarin.Forms.Build.Tasks
{
	class TypeRefComparer : IEqualityComparer<TypeReference>
	{
		static string GetAssembly(TypeReference typeRef)
		{
			if (typeRef.Scope is ModuleDefinition md)
				return md.Assembly.FullName;
			if (typeRef.Scope is AssemblyNameReference anr)
				return anr.FullName;
			throw new ArgumentOutOfRangeException(nameof(typeRef));
		}

		public bool Equals(TypeReference x, TypeReference y)
		{
			if (x == null)
				return y == null;
			if (y == null)
				return x == null;

			//strip the leading `&` as byref typered fullnames have a `&`
			var xname = x.FullName.EndsWith("&", StringComparison.InvariantCulture) ? x.FullName.Substring(0, x.FullName.Length - 1) : x.FullName;
			var yname = y.FullName.EndsWith("&", StringComparison.InvariantCulture) ? y.FullName.Substring(0, y.FullName.Length - 1) : y.FullName;
			if (xname != yname)
				return false;
			var xasm = GetAssembly(x);
			var yasm = GetAssembly(y);

			//standard types comes from either mscorlib. System.Runtime or netstandard. Assume they are equivalent
			if ((      xasm.StartsWith("System.Runtime", StringComparison.Ordinal)
					|| xasm.StartsWith("System", StringComparison.Ordinal)
					|| xasm.StartsWith("mscorlib", StringComparison.Ordinal)
					|| xasm.StartsWith("netstandard", StringComparison.Ordinal)
					|| xasm.StartsWith("System.Xml", StringComparison.Ordinal))
				&& (   yasm.StartsWith("System.Runtime", StringComparison.Ordinal)
					|| yasm.StartsWith("System", StringComparison.Ordinal)
					|| yasm.StartsWith("mscorlib", StringComparison.Ordinal)
					|| yasm.StartsWith("netstandard", StringComparison.Ordinal)
					|| yasm.StartsWith("System.Xml", StringComparison.Ordinal)))
				return true;
			return xasm == yasm;
		}

		public int GetHashCode(TypeReference obj)
		{
			return $"{GetAssembly(obj)}//{obj.FullName}".GetHashCode();
		}

		static TypeRefComparer s_default;
		public static TypeRefComparer Default => s_default ?? (s_default = new TypeRefComparer());
	}

	static class TypeReferenceExtensions
	{
		public static PropertyDefinition GetProperty(this TypeReference typeRef, Func<PropertyDefinition, bool> predicate,
			out TypeReference declaringTypeRef)
		{
			declaringTypeRef = typeRef;
			var typeDef = typeRef.ResolveCached();
			var properties = typeDef.Properties.Where(predicate);
			if (properties.Any())
				return properties.Single();
			if (typeDef.IsInterface) {
				foreach (var face in typeDef.Interfaces) {
					var p = face.InterfaceType.ResolveGenericParameters(typeRef).GetProperty(predicate, out var interfaceDeclaringTypeRef);
					if (p != null) {
						declaringTypeRef = interfaceDeclaringTypeRef;
						return p;
					}
				}
			}
			if (typeDef.BaseType == null || typeDef.BaseType.FullName == "System.Object")
				return null;
			return typeDef.BaseType.ResolveGenericParameters(typeRef).GetProperty(predicate, out declaringTypeRef);
		}

		public static EventDefinition GetEvent(this TypeReference typeRef, Func<EventDefinition, bool> predicate,
			out TypeReference declaringTypeRef)
		{
			declaringTypeRef = typeRef;
			var typeDef = typeRef.ResolveCached();
			var events = typeDef.Events.Where(predicate);
			if (events.Any()) {
				var ev = events.Single();
				return ev.ResolveGenericEvent(declaringTypeRef);
			}
			if (typeDef.BaseType == null || typeDef.BaseType.FullName == "System.Object")
				return null;
			return typeDef.BaseType.ResolveGenericParameters(typeRef).GetEvent(predicate, out declaringTypeRef);
		}

		//this resolves generic eventargs (https://bugzilla.xamarin.com/show_bug.cgi?id=57574)
		static EventDefinition ResolveGenericEvent(this EventDefinition eventDef, TypeReference declaringTypeRef)
		{
			if (eventDef == null)
				throw new ArgumentNullException(nameof(eventDef));
			if (declaringTypeRef == null)
				throw new ArgumentNullException(nameof(declaringTypeRef));
			if (!eventDef.EventType.IsGenericInstance)
				return eventDef;
			if (eventDef.EventType.ResolveCached().FullName != "System.EventHandler`1")
				return eventDef;

			var git = eventDef.EventType as GenericInstanceType;
			var ga = git.GenericArguments.First();
			ga = ga.ResolveGenericParameters(declaringTypeRef);
			git.GenericArguments[0] = ga;
			eventDef.EventType = git;

			return eventDef;

		}
		public static FieldDefinition GetField(this TypeReference typeRef, Func<FieldDefinition, bool> predicate,
			out TypeReference declaringTypeRef)
		{
			declaringTypeRef = typeRef;
			var typeDef = typeRef.ResolveCached();
			var bp = typeDef.Fields.Where
				(predicate);
			if (bp.Any())
				return bp.Single();
			if (typeDef.BaseType == null || typeDef.BaseType.FullName == "System.Object")
				return null;
			return typeDef.BaseType.ResolveGenericParameters(typeRef).GetField(predicate, out declaringTypeRef);
		}

		public static bool ImplementsInterface(this TypeReference typeRef, TypeReference @interface)
		{
			var typeDef = typeRef.ResolveCached();
			if (typeDef.Interfaces.Any(tr => tr.InterfaceType.FullName == @interface.FullName))
				return true;
			var baseTypeRef = typeDef.BaseType;
			if (baseTypeRef != null && baseTypeRef.FullName != "System.Object")
				return baseTypeRef.ImplementsInterface(@interface);
			return false;
		}

		public static bool ImplementsGenericInterface(this TypeReference typeRef, string @interface,
			out GenericInstanceType interfaceReference, out IList<TypeReference> genericArguments)
		{
			interfaceReference = null;
			genericArguments = null;
			var typeDef = typeRef.ResolveCached();
			InterfaceImplementation iface;
			if ((iface = typeDef.Interfaces.FirstOrDefault(tr =>
							tr.InterfaceType.FullName.StartsWith(@interface, StringComparison.Ordinal) &&
							tr.InterfaceType.IsGenericInstance && (tr.InterfaceType as GenericInstanceType).HasGenericArguments)) != null)
			{
				interfaceReference = (iface.InterfaceType as GenericInstanceType).ResolveGenericParameters(typeRef);
				genericArguments = interfaceReference.GenericArguments;
				return true;
			}
			var baseTypeRef = typeDef.BaseType;
			if (baseTypeRef != null && baseTypeRef.FullName != "System.Object")
				return baseTypeRef.ResolveGenericParameters(typeRef).ImplementsGenericInterface(@interface, out interfaceReference, out genericArguments);
			return false;
		}

		static readonly string[] arrayInterfaces = {
			"System.ICloneable",
			"System.Collections.IEnumerable",
			"System.Collections.IList",
			"System.Collections.ICollection",
			"System.Collections.IStructuralComparable",
			"System.Collections.IStructuralEquatable",
		};

		static readonly string[] arrayGenericInterfaces = {
			"System.Collections.Generic.IEnumerable`1",
			"System.Collections.Generic.IList`1",
			"System.Collections.Generic.ICollection`1",
			"System.Collections.Generic.IReadOnlyCollection`1",
			"System.Collections.Generic.IReadOnlyList`1",
		};

		public static bool InheritsFromOrImplements(this TypeReference typeRef, TypeReference baseClass)
		{
			if (TypeRefComparer.Default.Equals(typeRef, baseClass))
				return true;

			if (typeRef.IsValueType)
				return false;

			if (typeRef.IsArray) {
				var array = (ArrayType)typeRef;
				var arrayType = typeRef.ResolveCached();
				if (arrayInterfaces.Contains(baseClass.FullName))
					return true;
				if (array.IsVector &&  //generic interfaces are not implemented on multidimensional arrays
				    arrayGenericInterfaces.Contains(baseClass.ResolveCached().FullName) &&
					baseClass.IsGenericInstance &&
					TypeRefComparer.Default.Equals((baseClass as GenericInstanceType).GenericArguments[0], arrayType))
					return true;
				return baseClass.FullName == "System.Object";
			}

			if (typeRef.FullName == "System.Object")
				return false;
			var typeDef = typeRef.ResolveCached();
			if (typeDef.Interfaces.Any(ir => TypeRefComparer.Default.Equals(ir.InterfaceType.ResolveGenericParameters(typeRef), baseClass)))
				return true;
			if (typeDef.BaseType == null)
				return false;

			typeRef = typeDef.BaseType.ResolveGenericParameters(typeRef);
			return typeRef.InheritsFromOrImplements(baseClass);
		}

		static CustomAttribute GetCustomAttribute(this TypeReference typeRef, TypeReference attribute)
		{
			var typeDef = typeRef.ResolveCached();
			//FIXME: avoid string comparison. make sure the attribute TypeRef is the same one
			var attr = typeDef.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == attribute.FullName);
			if (attr != null)
				return attr;
			var baseTypeRef = typeDef.BaseType;
			if (baseTypeRef != null && baseTypeRef.FullName != "System.Object")
				return baseTypeRef.GetCustomAttribute(attribute);
			return null;
		}

		public static CustomAttribute GetCustomAttribute(this TypeReference typeRef, ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) attributeType)
		{
			return typeRef.GetCustomAttribute(module.ImportReference(attributeType));
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static MethodDefinition GetMethod(this TypeReference typeRef, Func<MethodDefinition, bool> predicate)
		{
			TypeReference declaringTypeReference;
			return typeRef.GetMethod(predicate, out declaringTypeReference);
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static MethodDefinition GetMethod(this TypeReference typeRef, Func<MethodDefinition, bool> predicate,
			out TypeReference declaringTypeRef)
		{
			declaringTypeRef = typeRef;
			var typeDef = typeRef.ResolveCached();
			var methods = typeDef.Methods.Where(predicate);
			if (methods.Any())
				return methods.Single();
			if (typeDef.BaseType != null && typeDef.BaseType.FullName == "System.Object")
				return null;
			if (typeDef.IsInterface)
			{
				foreach (var face in typeDef.Interfaces)
				{
					var m = face.InterfaceType.GetMethod(predicate);
					if (m != null)
						return m;
				}
				return null;
			}
			return typeDef.BaseType.GetMethod(predicate, out declaringTypeRef);
		}

		public static IEnumerable<Tuple<MethodDefinition, TypeReference>> GetMethods(this TypeReference typeRef,
			Func<MethodDefinition, bool> predicate, ModuleDefinition module)
		{
			return typeRef.GetMethods((md, tr) => predicate(md), module);
		}

		public static IEnumerable<Tuple<MethodDefinition, TypeReference>> GetMethods(this TypeReference typeRef,
			Func<MethodDefinition, TypeReference, bool> predicate, ModuleDefinition module)
		{
			var typeDef = typeRef.ResolveCached();
			foreach (var method in typeDef.Methods.Where(md => predicate(md, typeRef)))
				yield return new Tuple<MethodDefinition, TypeReference>(method, typeRef);
			if (typeDef.IsInterface)
			{
				foreach (var face in typeDef.Interfaces)
				{
					if (face.InterfaceType.IsGenericInstance && typeRef is GenericInstanceType)
					{
						int i = 0;
						foreach (var arg in ((GenericInstanceType)typeRef).GenericArguments)
							((GenericInstanceType)face.InterfaceType).GenericArguments[i++] = module.ImportReference(arg);
					}
					foreach (var tuple in face.InterfaceType.GetMethods(predicate, module))
						yield return tuple;
				}
				yield break;
			}
			if (typeDef.BaseType == null || typeDef.BaseType.FullName == "System.Object")
				yield break;
			var baseType = typeDef.BaseType.ResolveGenericParameters(typeRef);
			foreach (var tuple in baseType.GetMethods(predicate, module))
				yield return tuple;
		}

		public static MethodReference GetImplicitOperatorTo(this TypeReference fromType, TypeReference toType, ModuleDefinition module)
		{
			if (TypeRefComparer.Default.Equals(fromType, toType))
				return null;

			var implicitOperatorsOnFromType = fromType.GetMethods(md =>    md.IsPublic
																		&& md.IsStatic
																		&& md.IsSpecialName
																		&& md.Name == "op_Implicit", module);
			var implicitOperatorsOnToType = toType.GetMethods(md =>    md.IsPublic
																	&& md.IsStatic
																	&& md.IsSpecialName
																	&& md.Name == "op_Implicit", module);
			var implicitOperators = implicitOperatorsOnFromType.Concat(implicitOperatorsOnToType).ToList();

			if (implicitOperators.Any()) {
				foreach (var op in implicitOperators) {
					var cast = op.Item1;
					var opDeclTypeRef = op.Item2;
					var castDef = module.ImportReference(cast).ResolveGenericParameters(opDeclTypeRef, module);
					var returnType = castDef.ReturnType;
					if (returnType.IsGenericParameter)
						returnType = ((GenericInstanceType)opDeclTypeRef).GenericArguments [((GenericParameter)returnType).Position];
					if (!returnType.InheritsFromOrImplements(toType))
						continue;
					var paramType = cast.Parameters[0].ParameterType.ResolveGenericParameters(castDef);
					if (!fromType.InheritsFromOrImplements(paramType))
						continue;
					return castDef;
				}
			}
			return null;
		}

		public static TypeReference ResolveGenericParameters(this TypeReference self, MethodReference declaringMethodReference)
		{
			var genericParameterSelf = self as GenericParameter;
			var genericdeclMethod = declaringMethodReference as GenericInstanceMethod;
			var declaringTypeReference = declaringMethodReference.DeclaringType;
			var genericdeclType = declaringTypeReference as GenericInstanceType;

			if (genericParameterSelf != null) {
				switch (genericParameterSelf.Type) {
				case GenericParameterType.Method:
					self = genericdeclMethod.GenericArguments[genericParameterSelf.Position];
					break;

				case GenericParameterType.Type:
					self = genericdeclType.GenericArguments[genericParameterSelf.Position];
					break;
				}
			}

			var genericself = self as GenericInstanceType;
			if (genericself == null)
				return self;

			genericself = genericself.ResolveGenericParameters(declaringTypeReference);
			for (var i = 0; i < genericself.GenericArguments.Count; i++) {
				var genericParameter = genericself.GenericArguments[i] as GenericParameter;
				if (genericParameter != null)
					genericself.GenericArguments[i] = genericdeclMethod.GenericArguments[genericParameter.Position];
			}
			return genericself;
		}

		public static TypeReference ResolveGenericParameters(this TypeReference self, TypeReference declaringTypeReference)
		{
			var genericdeclType = declaringTypeReference as GenericInstanceType;
			if (genericdeclType == null)
				return self;

			var genericParameterSelf = self as GenericParameter;
			if (genericParameterSelf != null)
				return genericdeclType.GenericArguments[genericParameterSelf.Position];

			var genericself = self as GenericInstanceType;
			if (genericself != null)
				return genericself.ResolveGenericParameters(declaringTypeReference);

			return self;
		}

		public static GenericInstanceType ResolveGenericParameters(this GenericInstanceType self, TypeReference declaringTypeReference)
		{
			var genericdeclType = declaringTypeReference as GenericInstanceType;
			if (genericdeclType == null)
				return self;

			return self.ResolveGenericParameters(genericdeclType);
		}

		public static GenericInstanceType ResolveGenericParameters(this GenericInstanceType self, GenericInstanceType declaringTypeReference)
		{
			List<TypeReference> args = new List<TypeReference>();
			for (var i = 0; i < self.GenericArguments.Count; i++) {
				var genericParameter = self.GenericArguments[i] as GenericParameter;
				if (genericParameter == null)
					args.Add(self.GenericArguments[i].ResolveGenericParameters(declaringTypeReference));
				else if (genericParameter.Type == GenericParameterType.Type)
					args.Add(declaringTypeReference.GenericArguments[genericParameter.Position]);
			}
			return self.ElementType.MakeGenericInstanceType(args.ToArray());
		}

		static Dictionary<TypeReference, TypeDefinition> resolves = new Dictionary<TypeReference, TypeDefinition>();
		public static TypeDefinition ResolveCached(this TypeReference typeReference)
		{
			if (resolves.TryGetValue(typeReference, out var typeDefinition))
				return typeDefinition;
			return (resolves[typeReference] = typeReference.Resolve());
		}
	}
}
