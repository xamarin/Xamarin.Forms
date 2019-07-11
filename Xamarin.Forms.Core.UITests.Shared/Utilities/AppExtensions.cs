﻿using System;
using System.Linq;

using Xamarin.UITest;
using Xamarin.UITest.Queries;
using System.Text.RegularExpressions;
using System.Threading;

namespace Xamarin.Forms.Core.UITests
{
	internal static class AppExtensions
	{
		public static T[] RetryUntilPresent<T>(
			this IApp app,
			Func<T[]> func,
			int retryCount = 10,
			int delayInMs = 2000)
		{
			var results = func();

			int counter = 0;
			while ((results == null || results.Length == 0) && counter < retryCount)
			{
				Thread.Sleep(delayInMs);
				results = func();
				counter++;
			}

			return results;
		}

		public static AppRect ScreenBounds(this IApp app)
		{
			return app.Query(Queries.Root()).First().Rect;
		}

		public static void NavigateBack(this IApp app)
		{
			app.Back();
		}

		public static void NavigateToGallery(this IApp app, string page)
		{
			NavigateToGallery(app, page, null);
		}

		public static void NavigateToGallery(this IApp app, string page, string visual)
		{
			const string goToTestButtonQuery = "* marked:'GoToTestButton'";

			app.WaitForElement(q => q.Raw(goToTestButtonQuery), "Timed out waiting for Go To Test button to appear", TimeSpan.FromMinutes(2));

			var text = Regex.Match(page, "'(?<text>[^']*)'").Groups["text"].Value;

			app.WaitForElement("SearchBar");
			app.EnterText(q => q.Raw("* marked:'SearchBar'"), text);
			app.DismissKeyboard();

			if (!String.IsNullOrWhiteSpace(visual))
			{
				app.ActivateContextMenu($"{text}AutomationId");
				app.Tap("Select Visual");
				app.Tap("Material");
			}
			else
			{
				app.Tap(q => q.Raw(goToTestButtonQuery));
			}

			app.WaitForNoElement(o => o.Raw(goToTestButtonQuery), "Timed out waiting for Go To Test button to disappear", TimeSpan.FromMinutes(2));
		}


		public static AppResult[] QueryNTimes(this IApp app, Func<AppQuery, AppQuery> elementQuery, int numberOfTries, Action onFail)
		{
			int tryCount = 0;
			var elements = app.Query(elementQuery);
			while (elements.Length == 0 && tryCount < numberOfTries)
			{
				elements = app.Query(elementQuery);
				tryCount++;
				if (elements.Length == 0 && onFail != null)
					onFail();
			}

			return elements;
		}
	}
}