﻿using System;
using System.Diagnostics;
using System.Resources;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.MultiTouch;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Remote;
using Xamarin.Forms.Xaml;
using Xamarin.UITest;
using OpenQA.Selenium.Appium;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Appium.Windows.Enums;
using System.Windows.Input;

namespace Xamarin.Forms.Core.UITests
{
	public class WindowsTestBase
	{
		protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
		protected static WindowsDriver<WindowsElement> Session;

		public static IApp ConfigureApp()
		{
			if (Session == null)
			{
				AppiumOptions options = new AppiumOptions();
				options.AddAdditionalCapability("app", "0d4424f6-1e29-4476-ac00-ba22c3789cb6_ph1m9x8skttmg!App");
				Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), options);
				Assert.IsNotNull(Session);
				Session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
				Reset();
			}

			return new WinDriverApp(Session);
		}

		internal static void HandleAppClosed(Exception ex)
		{
			if (ex is InvalidOperationException && ex.Message == "Currently selected window has been closed")
			{
				Session = null;
			}
		}

		public static void Reset()
		{
			try
			{
				new Actions(Session)
					.SendKeys(Keys.Escape)
					.Perform();
			}
			catch (Exception ex)
			{
				HandleAppClosed(ex);
				Debug.WriteLine($">>>>> WindowsTestBase ConfigureApp 49: {ex}");
				throw;
			}
		}
	}
}
