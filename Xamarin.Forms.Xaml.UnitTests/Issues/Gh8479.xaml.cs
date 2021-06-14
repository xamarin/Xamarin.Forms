using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public class Gh8479Item
	{
		public string Value { get; set; }
	}
	public class Gh8479VM
	{
		public ObservableCollection<Gh8479Item> Items { get; set; }
	}

	public partial class Gh8479 : ContentPage
	{
		public Gh8479() => InitializeComponent();
		public Gh8479(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();

			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[Test]
			public void BindingIndexerOnObservable([Values(false, true)] bool useCompiledXaml)
			{
				var layout = new Gh8479(useCompiledXaml) {
					BindingContext = new Gh8479VM { Items = new ObservableCollection<Gh8479Item> ()}
				};
				var vm = (Gh8479VM)layout.BindingContext;
				vm.Items.Add(new Gh8479Item { Value = "foo" });
				vm.Items.Add(new Gh8479Item { Value = "bar" });
				vm.Items.Add(new Gh8479Item { Value = "baz" });
			
				Assert.That(layout.label0.Text, Is.EqualTo("bar"));
			}
		}
	}
}
