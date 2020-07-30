﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using NUnit.Framework;
using UIKit;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{
	[TestFixture]
	public class ImageButtonTests : PlatformTestFixture
	{
		[Test, Category("ImageButton")]
		public async Task CreatedWithCorrectButtonType()
		{
			var imageButton = new ImageButton();
			UIButtonType buttonType = await GetControlProperty(imageButton, uiButton => uiButton.ButtonType);
			Assert.AreNotEqual(UIButtonType.Custom, buttonType);
		}
	}
}