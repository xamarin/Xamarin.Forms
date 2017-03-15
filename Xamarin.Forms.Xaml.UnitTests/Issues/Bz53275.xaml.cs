using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Xamarin.Forms.Core.UnitTests;
using System.Reflection;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

	public partial class Bz53275 : ContentPage
	{
		public Bz53275()
		{
			InitializeComponent();
		}

		public Bz53275(bool useCompiledXaml)
		{

		}
	}

	[TestFixture]
	class Bz53275Test:BaseTestFixture
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void TestGetTargetProperty(bool useCompiledXaml)
		{
			var label = (new Bz53275(useCompiledXaml)).FindByName<Label>("label");
			Assert.AreEqual("Text", label.Text.ToString());
		}
	}
}
