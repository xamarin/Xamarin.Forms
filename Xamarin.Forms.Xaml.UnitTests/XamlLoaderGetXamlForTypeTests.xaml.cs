﻿using System;
using System.Collections.Generic;

using NUnit.Framework;

using Xamarin.Forms;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class XamlLoaderGetXamlForTypeTests : ContentPage
	{
		public XamlLoaderGetXamlForTypeTests()
		{
			InitializeComponent();
		}

		public XamlLoaderGetXamlForTypeTests(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[SetUp]
			public void SetUp()
			{
				XamlLoader.XamlFileProvider = null;
			}

			[TestCase(false)]
			[TestCase(true)]
			public void XamlContentIsReplaced(bool useCompiledXaml)
			{
				var layout = new XamlLoaderGetXamlForTypeTests(useCompiledXaml);
				Assert.That(layout.Content, Is.TypeOf<Button>());

				XamlLoader.XamlFileProvider = (t) => {
					if (t == typeof(XamlLoaderGetXamlForTypeTests))
						return @"
	<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
		xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
		x:Class=""Xamarin.Forms.Xaml.UnitTests.XamlLoaderGetXamlForTypeTests"">
		<Label x:Name=""Label""/>
	</ContentPage>";
					return null;
				};

				layout = new XamlLoaderGetXamlForTypeTests(useCompiledXaml);
				Assert.That(layout.Content, Is.TypeOf<Label>());
			}
		}
	}
}

