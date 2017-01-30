using System;
using System.Linq;

using Mono.Cecil;

using Xamarin.Forms.Build.Tasks;

using NUnit.Framework;
using System.Collections.Generic;

namespace Xamarin.Forms.Xaml.XamlcUnitTests
{
	[TestFixture]
	public class MethodReferenceExtensionsTests
	{
		ModuleDefinition module;

		[SetUp]
		public void SetUp ()
		{
			var resolver = new XamlCAssemblyResolver();
			resolver.AddAssembly(Uri.UnescapeDataString((new UriBuilder(typeof(MethodReferenceExtensionsTests).Assembly.CodeBase)).Path));
			resolver.AddAssembly(Uri.UnescapeDataString((new UriBuilder(typeof(BindableObject).Assembly.CodeBase)).Path));

			module = ModuleDefinition.CreateModule("foo", new ModuleParameters {
				AssemblyResolver = resolver,
				Kind = ModuleKind.NetModule
			});
		}

		[Test]
		public void ResolveRowDefAdder ()
		{
			var propertyType = module.Import(typeof (RowDefinitionCollection));
			var adderT = propertyType.GetMethods ((md, tr) => md.Name == "Add", module).Single ();
			var adder = adderT.Item1;
			var ptype = adderT.Item2;
			Assert.AreEqual ("System.Void Xamarin.Forms.DefinitionCollection`1::Add(T)", adder.FullName);
			Assert.AreEqual ("Xamarin.Forms.DefinitionCollection`1<Xamarin.Forms.RowDefinition>", ptype.FullName);
			var adderRef = module.Import (adder);
			Assert.AreEqual ("System.Void Xamarin.Forms.DefinitionCollection`1::Add(T)", adderRef.FullName);
			adderRef = adderRef.ResolveGenericParameters (ptype, module);
			Assert.AreEqual ("System.Void Xamarin.Forms.DefinitionCollection`1<Xamarin.Forms.RowDefinition>::Add(T)", adderRef.FullName);
		}

		[Test]
		public void GenericGetter ()
		{
			TypeReference declaringTypeReference;
			var type = module.Import (typeof (StackLayout));
			var property = type.GetProperty (pd => pd.Name == "Children", out declaringTypeReference);
			Assert.AreEqual ("System.Collections.Generic.IList`1<T> Xamarin.Forms.Layout`1::Children()", property.FullName);
			Assert.AreEqual ("Xamarin.Forms.Layout`1<Xamarin.Forms.View>", declaringTypeReference.FullName);
			var propertyGetter = property.GetMethod;
			Assert.AreEqual ("System.Collections.Generic.IList`1<T> Xamarin.Forms.Layout`1::get_Children()", propertyGetter.FullName);
			var propertyGetterRef = module.Import (propertyGetter);
			Assert.AreEqual ("System.Collections.Generic.IList`1<T> Xamarin.Forms.Layout`1::get_Children()", propertyGetterRef.FullName);

			propertyGetterRef = module.Import (propertyGetterRef.ResolveGenericParameters (declaringTypeReference, module));
			Assert.AreEqual ("System.Collections.Generic.IList`1<T> Xamarin.Forms.Layout`1<Xamarin.Forms.View>::get_Children()", propertyGetterRef.FullName);
			var returnType = propertyGetterRef.ReturnType.ResolveGenericParameters (declaringTypeReference);
			Assert.AreEqual ("System.Collections.Generic.IList`1<Xamarin.Forms.View>", returnType.FullName);
		}

		[Test]
		public void GetterWithGenericReturnType ()
		{
			TypeReference declaringTypeReference;
			var type = module.Import (typeof (Style));
			var property = type.GetProperty (pd => pd.Name == "Setters", out declaringTypeReference);
			Assert.AreEqual ("System.Collections.Generic.IList`1<Xamarin.Forms.Setter> Xamarin.Forms.Style::Setters()", property.FullName);
			Assert.AreEqual ("Xamarin.Forms.Style", declaringTypeReference.FullName);
			var propertyGetter = property.GetMethod;
			Assert.AreEqual ("System.Collections.Generic.IList`1<Xamarin.Forms.Setter> Xamarin.Forms.Style::get_Setters()", propertyGetter.FullName);

			var propertyGetterRef = module.Import (propertyGetter);
			Assert.AreEqual ("System.Collections.Generic.IList`1<Xamarin.Forms.Setter> Xamarin.Forms.Style::get_Setters()", propertyGetterRef.FullName);
			propertyGetterRef = module.Import (propertyGetterRef.ResolveGenericParameters (declaringTypeReference, module));
			Assert.AreEqual ("System.Collections.Generic.IList`1<Xamarin.Forms.Setter> Xamarin.Forms.Style::get_Setters()", propertyGetterRef.FullName);
			var returnType = propertyGetterRef.ReturnType.ResolveGenericParameters (declaringTypeReference);
			Assert.AreEqual ("System.Collections.Generic.IList`1<Xamarin.Forms.Setter>", returnType.FullName);
		}

		[Test]
		public void ResolveChildren ()
		{
			var propertyType = module.Import (typeof (IList<View>));
			var adderT = propertyType.GetMethods (md => md.Name == "Add" && md.Parameters.Count == 1, module).Single ();
			var adder = adderT.Item1;
			var ptype = adderT.Item2;
			Assert.AreEqual ("System.Void System.Collections.Generic.ICollection`1::Add(T)", adder.FullName);
			Assert.AreEqual ("System.Collections.Generic.ICollection`1<Xamarin.Forms.View>", ptype.FullName);
			var adderRef = module.Import (adder);
			Assert.AreEqual ("System.Void System.Collections.Generic.ICollection`1::Add(T)", adderRef.FullName);
			adderRef = adderRef.ResolveGenericParameters (ptype, module);
			Assert.AreEqual ("System.Void System.Collections.Generic.ICollection`1<Xamarin.Forms.View>::Add(T)", adderRef.FullName);
		}
	}
}