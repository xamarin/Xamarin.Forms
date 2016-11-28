using System;
using NUnit.Framework;
using Mono.Cecil;
using Xamarin.Forms.Build.Tasks;
using System.Collections.Generic;

namespace Xamarin.Forms.Xaml.XamlcUnitTests
{
	[TestFixture]
	public class TypeReferenceExtensionsTests
	{
		ModuleDefinition module;

		[SetUp]
		public void SetUp()
		{
			var resolver = new XamlCAssemblyResolver();
			resolver.AddAssembly(Uri.UnescapeDataString((new UriBuilder(typeof(TypeReferenceExtensionsTests).Assembly.CodeBase)).Path));
			resolver.AddAssembly(Uri.UnescapeDataString((new UriBuilder(typeof(BindableObject).Assembly.CodeBase)).Path));
			resolver.AddAssembly(Uri.UnescapeDataString((new UriBuilder(typeof(object).Assembly.CodeBase)).Path));
			resolver.AddAssembly(Uri.UnescapeDataString((new UriBuilder(typeof(IList<>).Assembly.CodeBase)).Path));
			resolver.AddAssembly(Uri.UnescapeDataString((new UriBuilder(typeof(Queue<>).Assembly.CodeBase)).Path));

			module = ModuleDefinition.CreateModule("foo", new ModuleParameters {
				AssemblyResolver = resolver,
				Kind = ModuleKind.NetModule
			});
		}

		[TestCase(typeof(bool), typeof(BindableObject), ExpectedResult = false)]
		[TestCase(typeof(System.Collections.Generic.Dictionary<System.String,System.String>), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(System.Collections.Generic.List<System.String>), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(System.Collections.Generic.List<Xamarin.Forms.Button>), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(System.Collections.Generic.Queue<System.Collections.Generic.KeyValuePair<System.String,System.String>>), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(System.Double), typeof(System.Double), ExpectedResult = true)]
		[TestCase(typeof(System.Object), typeof(System.Collections.Generic.IList<Xamarin.Forms.TriggerBase>), ExpectedResult = false)]
		[TestCase(typeof(System.Object), typeof(System.Double), ExpectedResult = false)]
		[TestCase(typeof(System.Object), typeof(System.Nullable<System.Int32>), ExpectedResult = false)]
		[TestCase(typeof(System.Object), typeof(System.Object), ExpectedResult = true)]
		[TestCase(typeof(System.SByte), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(System.String[]), typeof(System.Collections.IEnumerable), ExpectedResult = true)]
		[TestCase(typeof(System.String[]), typeof(System.Object), ExpectedResult = true)]
		[TestCase(typeof(System.String[]), typeof(Xamarin.Forms.BindingBase), ExpectedResult = false)]
		[TestCase(typeof(System.String[]), typeof(System.Collections.Generic.IEnumerable<string>), ExpectedResult = true)]
		[TestCase(typeof(System.Type), typeof(System.Object), ExpectedResult = true)]
		[TestCase(typeof(System.Type), typeof(System.Type), ExpectedResult = true)]
		[TestCase(typeof(System.Type), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(System.Windows.Input.ICommand), typeof(System.Windows.Input.ICommand), ExpectedResult = true)]
		[TestCase(typeof(System.Windows.Input.ICommand), typeof(Xamarin.Forms.BindingBase), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.BindingBase), typeof(Xamarin.Forms.BindingBase), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.BindingCondition), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.Button), typeof(Xamarin.Forms.BindableObject), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.Button), typeof(Xamarin.Forms.BindingBase), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.Button), typeof(Xamarin.Forms.View), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.Color), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.Color), typeof(Xamarin.Forms.BindingBase), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.Color), typeof(Xamarin.Forms.Color), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.ColumnDefinition), typeof(Xamarin.Forms.BindableObject), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.ColumnDefinition), typeof(Xamarin.Forms.BindingBase), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.ColumnDefinition), typeof(Xamarin.Forms.ColumnDefinitionCollection), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.Constraint), typeof(Xamarin.Forms.BindingBase), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.Constraint), typeof(Xamarin.Forms.Constraint), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.ConstraintExpression), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.ContentPage), typeof(Xamarin.Forms.BindableObject), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.ContentPage), typeof(Xamarin.Forms.Page), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.ContentView), typeof(Xamarin.Forms.BindableObject), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.ContentView[]), typeof(System.Collections.Generic.IList<Xamarin.Forms.ContentView>), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.MultiTrigger), typeof(System.Collections.Generic.IList<Xamarin.Forms.TriggerBase>), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.OnIdiom<System.Double>), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.OnPlatform<System.String>), typeof(System.String), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.OnPlatform<System.String>), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.OnPlatform<System.String>), typeof(Xamarin.Forms.BindingBase), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.OnPlatform<Xamarin.Forms.FontAttributes>), typeof(Xamarin.Forms.BindableObject), ExpectedResult = false)]
		[TestCase(typeof(Xamarin.Forms.StackLayout), typeof(Xamarin.Forms.Layout<Xamarin.Forms.View>), ExpectedResult = true)]
		[TestCase(typeof(Xamarin.Forms.StackLayout), typeof(Xamarin.Forms.View), ExpectedResult = true)]
		public bool TestInheritsFromOrImplements(Type typeRef, Type baseClass)
		{
			return TypeReferenceExtensions.InheritsFromOrImplements(module.Import(typeRef), module.Import(baseClass));
		}
	}
}