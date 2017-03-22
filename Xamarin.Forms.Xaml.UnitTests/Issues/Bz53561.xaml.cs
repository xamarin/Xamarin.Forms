using System;
using System.Collections.Generic;
using Xamarin.Forms;
using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Bz53561 : ContentPage
	{
		public Bz53561()
		{
			InitializeComponent();
		}

		public Bz53561(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[TestCase(true)]
			[TestCase(true)]
			public void ConverterParameterOrderDoesNotMatters(bool useCompiledXaml)
			{
				Bz53561 layout = new Bz53561(useCompiledXaml);
				Assert.AreEqual(layout.Height, layout.Label1Element.Height + layout.Label2Element.Height);
				Assert.AreEqual(layout.Label2Element.Height, layout.GridElement.RowDefinitions[1].ActualHeight);
			}
		}
	}
}
