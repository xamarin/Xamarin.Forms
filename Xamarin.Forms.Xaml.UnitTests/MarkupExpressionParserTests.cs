using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[TestFixture]
	public class MarkupExpressionParserTests : BaseTestFixture
	{
		IXamlTypeResolver typeResolver;

		class MockElementNode : IElementNode, IValueNode, IXmlLineInfo
		{
			public bool HasLineInfo () { return false; }

			public int LineNumber {
				get { return -1; }
			}

			public int LinePosition {
				get { return -1; }
			}


			public IXmlNamespaceResolver NamespaceResolver {
				get {
					throw new NotImplementedException ();
				}
			}

			public object Value {get;set;}
			public Dictionary<XmlName, INode> Properties { get; set; }

			public List<XmlName> SkipProperties { get; set; }

			public Forms.Internals.INameScope Namescope {
				get {
					throw new NotImplementedException ();
				}
			}

			public XmlType XmlType {
				get;
				set;
			}

			public string NamespaceURI {
				get {
					throw new NotImplementedException ();
				}
			}

			public INode Parent {
				get {
					throw new NotImplementedException ();
				}
				set { throw new NotImplementedException (); }
			}
				
			public List<INode> CollectionItems { get; set; }

			public void Accept (IXamlNodeVisitor visitor, INode parentNode)
			{
				throw new NotImplementedException ();
			}


			public List<string> IgnorablePrefixes { get; set; }

			public INode Clone()
			{
				throw new NotImplementedException();
			}
		}

		[SetUp]
		public override void Setup ()
		{
			base.Setup ();
			var nsManager = new XmlNamespaceManager (new NameTable ());
			nsManager.AddNamespace ("local", "clr-namespace:Xamarin.Forms.Xaml.UnitTests;assembly=Xamarin.Forms.Xaml.UnitTests");
			nsManager.AddNamespace ("x", "http://schemas.microsoft.com/winfx/2009/xaml");
			typeResolver = new Internals.XamlTypeResolver (nsManager, XamlParser.GetElementType, Assembly.GetCallingAssembly ());
		}

		[Test]
		public void BindingOnSelf ()
		{
			var bindingString = "{Binding}";
			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});
			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual (Binding.SelfPath, ((Binding)binding).Path);
		}

		[Test]
		public void BindingWithImplicitPath ()
		{
			var bindingString = "{Binding Foo}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
		}

		[Test]
		public void BindingWithPath ()
		{
			var bindingString = "{Binding Path=Foo}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
		}

		[Test]
		public void BindingWithComposedPath ()
		{
			var bindingString = "{Binding Path=Foo.Bar}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo.Bar", ((Binding)binding).Path);
		}

		[Test]
		public void BindingWithImplicitComposedPath ()
		{
			var bindingString = "{Binding Path=Foo.Bar}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo.Bar", ((Binding)binding).Path);
		}

		class MockValueProvider : IProvideParentValues, IProvideValueTarget
		{
			public MockValueProvider (string key, object resource)
			{
				var rd = new ResourceDictionary {
					{key, resource}
				};

				ve = new VisualElement {
					Resources = rd,
				};
			}


			VisualElement ve;
			public IEnumerable<object> ParentObjects {
				get {
					yield return ve;
				}
			}

			public object TargetObject {
				get {
					throw new NotImplementedException ();
				}
			}

			public object TargetProperty { get; } = null;
		}

		[Test]
		public void BindingWithImplicitPathAndConverter ()
		{
			var bindingString = "{Binding Foo, Converter={StaticResource Bar}}";
			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
				IProvideValueTarget = new MockValueProvider ("Bar", new ReverseConverter()),
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.NotNull (((Binding)binding).Converter);
			Assert.That (((Binding)binding).Converter, Is.InstanceOf<ReverseConverter> ());
		}

		[Test]
		public void BindingWithPathAndConverter ()
		{
			var bindingString = "{Binding Path=Foo, Converter={StaticResource Bar}}";
			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
				IProvideValueTarget = new MockValueProvider ("Bar", new ReverseConverter()),
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.NotNull (((Binding)binding).Converter);
			Assert.That (((Binding)binding).Converter, Is.InstanceOf<ReverseConverter> ());
		}


		[Test]
		public void TestBindingMode ()
		{
			var bindingString = "{Binding Foo, Mode=TwoWay}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.AreEqual (BindingMode.TwoWay, ((Binding)binding).Mode);
		}

		[Test]
		public void BindingStringFormat ()
		{
			var bindingString = "{Binding Foo, StringFormat=Bar}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});
			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.AreEqual ("Bar", ((Binding)binding).StringFormat);		
		}

		[Test]
		public void BindingStringFormatWithEscapes ()
		{
			var bindingString = "{Binding Foo, StringFormat='{}Hello {0}'}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.AreEqual ("Hello {0}", ((Binding)binding).StringFormat);		
		}

		[Test]
		public void BindingStringFormatWithoutEscaping ()
		{
			var bindingString = "{Binding Foo, StringFormat='{0,20}'}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.AreEqual ("{0,20}", ((Binding)binding).StringFormat);
		}

		[Test]
		public void BindingStringFormatNumeric ()
		{
			var bindingString = "{Binding Foo, StringFormat=P2}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.AreEqual ("P2", ((Binding)binding).StringFormat);	
		}

		[Test]
		public void BindingConverterParameter ()
		{
			var bindingString = "{Binding Foo, ConverterParameter='Bar'}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo", ((Binding)binding).Path);
			Assert.AreEqual ("Bar", ((Binding)binding).ConverterParameter);	
		}

		[Test]
		public void BindingsCompleteString ()
		{
			var bindingString = "{Binding Path=Foo.Bar, StringFormat='{}Qux, {0}', Converter={StaticResource Baz}, Mode=OneWayToSource}";
			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
				IProvideValueTarget = new MockValueProvider ("Baz", new ReverseConverter()),
			});

			Assert.That (binding, Is.InstanceOf<Binding> ());
			Assert.AreEqual ("Foo.Bar", ((Binding)binding).Path);
			Assert.NotNull (((Binding)binding).Converter);
			Assert.That (((Binding)binding).Converter, Is.InstanceOf<ReverseConverter> ());
			Assert.AreEqual (BindingMode.OneWayToSource, ((Binding)binding).Mode);
			Assert.AreEqual ("Qux, {0}", ((Binding)binding).StringFormat);	
		}

		[Test]
		public void BindingWithStaticConverter ()
		{
			var bindingString = "{Binding Converter={x:Static local:ReverseConverter.Instance}}";

			var binding = (new MarkupExtensionParser ()).ParseExpression (ref bindingString, new Internals.XamlServiceProvider (null, null) {
				IXamlTypeResolver = typeResolver,
			}) as Binding;
				
			Assert.NotNull (binding);
			Assert.AreEqual(".", binding.Path);
			Assert.That (binding.Converter, Is.TypeOf<ReverseConverter> ());
		}

		[TestCase("{OnPlatform Android=23, Default=20}", Device.Android, "23")]
		[TestCase("{OnPlatform Android=20, iOS=25}", Device.iOS, "25")]
		[TestCase("{OnPlatform Android=20, GTK=25}", Device.GTK, "25")]
		[TestCase("{OnPlatform Android=20, macOS=25}", Device.macOS, "25")]
		[TestCase("{OnPlatform Android=20, Tizen=25}", Device.Tizen, "25")]
		[TestCase("{OnPlatform Android=20, UWP=25}", Device.UWP, "25")]
		[TestCase("{OnPlatform Android=20, WPF=25}", Device.WPF, "25")]
		[TestCase("{OnPlatform Android=23, Default=20}", Device.iOS, "20")]
		[TestCase("{OnPlatform Android=23, Default=20}", Device.GTK, "20")]
		[TestCase("{OnPlatform Android=23, Default=20}", Device.macOS, "20")]
		[TestCase("{OnPlatform Android=23, Default=20}", Device.Tizen, "20")]
		[TestCase("{OnPlatform Android=23, Default=20}", Device.UWP, "20")]
		[TestCase("{OnPlatform Android=23, Default=20}", Device.WPF, "20")]
		[TestCase("{OnPlatform Android=23, Default=20}", "Foo", "20")]
		[TestCase("{OnPlatform Android=23, Default=20, Other=Foo:10}", "Bar", "20")]
		[TestCase("{OnPlatform Android=23, Default=20, Other=Foo:10;Bar:15;Baz:18}", "Foo", "10")]
		[TestCase("{OnPlatform Android=23, Default=20, Other=Foo:10;Bar:15;Baz:18}", "Bar", "15")]
		[TestCase("{OnPlatform Android=23, Default=20, Other=Foo:10;Bar:15;Baz:18}", "Baz", "18")]
		public void OnPlatformExtension(string markup, string platform, string expected)
		{
			var services = new MockPlatformServices
			{
				RuntimePlatform = platform
			};
			Device.PlatformServices = services;

			var actual = (new MarkupExtensionParser()).ParseExpression(ref markup, new Internals.XamlServiceProvider(null, null)
			{
				IXamlTypeResolver = typeResolver,
			});

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void OnIdiomExtension()
		{
			//var services = new MockPlatformServices();
			//Device.PlatformServices = services;
			Func<string, TargetIdiom, object> parse = (markup, idiom) =>
			{
				Device.SetIdiom(idiom);
				return (new MarkupExtensionParser()).ParseExpression(ref markup, new Internals.XamlServiceProvider(null, null)
				{
					IXamlTypeResolver = typeResolver,
				});
			};

			Assert.AreEqual("23", parse("{OnIdiom Phone=23, Tablet=25, Default=20}", TargetIdiom.Phone));
			Assert.AreEqual("25", parse("{OnIdiom Phone=23, Tablet=25, Default=20}", TargetIdiom.Tablet));
			Assert.AreEqual("20", parse("{OnIdiom Phone=23, Tablet=25, Default=20}", TargetIdiom.Desktop));
			Assert.AreEqual("26", parse("{OnIdiom Phone=23, Tablet=25, Desktop=26, TV=30, Watch=10}", TargetIdiom.Desktop));
			Assert.AreEqual("30", parse("{OnIdiom Phone=23, Tablet=25, Desktop=26, TV=30, Watch=10}", TargetIdiom.TV));
			Assert.AreEqual("10", parse("{OnIdiom Phone=23, Tablet=25, Desktop=26, TV=30, Watch=10}", TargetIdiom.Watch));
			Assert.AreEqual("0", parse("{OnIdiom Phone=23, Unsupported=0}", TargetIdiom.Unsupported));
			Assert.AreEqual(default(string), parse("{OnIdiom Phone=23}", TargetIdiom.Desktop));
		}
	}
}
