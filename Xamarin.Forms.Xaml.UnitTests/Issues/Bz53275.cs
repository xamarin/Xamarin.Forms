using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml;

using Xamarin.Forms.Core.UnitTests;
using System.Reflection;

namespace Xamarin.Forms.Xaml.UnitTests.Issues
{
	class GetTargetPropertyName : IMarkupExtension
	{
		public object ProvideValue(IServiceProvider provider)
		{

			if (provider == null) return false;

			//create a binding and assign it to the target
			var service = provider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (service == null) return false;

			//we need dependency objects / properties
			var propertyName = string.Empty;
			var property = service.TargetProperty;
			if (property is BindableProperty)
			{
				propertyName = ((BindableProperty)property).PropertyName;
			}
			else if(property is PropertyInfo)
			{
				propertyName = ((PropertyInfo)property).Name;
			}
			return propertyName;
		}
	}

	[TestFixture]
	class Bz53275:BaseTestFixture
	{
		IXamlTypeResolver typeResolver;

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			var nsManager = new XmlNamespaceManager(new NameTable());
			nsManager.AddNamespace("local", "clr-namespace:Xamarin.Forms.Xaml.UnitTests.Issues;assembly=Xamarin.Forms.Xaml.UnitTests");
			nsManager.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");

			typeResolver = new Internals.XamlTypeResolver(nsManager, XamlParser.GetElementType, Assembly.GetCallingAssembly());
		}

		[Test]
		public void TestGetTargetProperty()
		{
			var xaml = @"
			<Label 
				xmlns=""http://xamarin.com/schemas/2014/forms""
				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
				xmlns:local=""clr-namespace:Xamarin.Forms.Xaml.UnitTests.Issues;assembly=Xamarin.Forms.Xaml.UnitTests""
				Text=""{local:GetTargetPropertyName}""
			/>";

			var label = new Label();
			label.LoadFromXaml(xaml);
			Assert.AreEqual("Text", label.Text.ToString());
		}
	}
}
