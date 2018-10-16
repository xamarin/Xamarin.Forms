//
// DesignTimeLoaderTests.cs
//
// Author:
//       Stephane Delcroix <stdelc@microsoft.com>
//
// Copyright (c) 2018 
using System;

using Xamarin.Forms;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public class DesignTimeLoaderTests : ContentPage
	{
		public DesignTimeLoaderTests()
		{
			Content = new StackLayout {
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}

