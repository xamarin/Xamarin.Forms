using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Forms.Core.Interactivity;

namespace Xamarin.Forms.Core.UnitTests
{
	class MockButtonTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is Button btn)
			{
				return btn.Text;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[TestFixture]
	public class EventToCommandBehaviorTests : BaseTestFixture
	{
		[Test]
		public void InvokesCommandWithNoParameter()
		{
			var button = new Button();
			bool wasClicked = false;
			var behavior = new EventToCommandBehavior
			{
				Command = new Command(() => wasClicked = true),
				EventName = "Clicked"
			};

			button.Behaviors.Add(behavior);
			button.SendClicked();

			Assert.True(wasClicked);
		}

		[Test]
		public void InvokesCommandWithParameterFromEventArgsPath()
		{
			var sl = new StackLayout();
			var button = new Button();
			Element element = null;
			var behavior = new EventToCommandBehavior
			{
				Command = new Command<Element>(e => element = e),
				EventName = "ChildAdded",
				EventArgsParameterPath = "Element"
			};

			sl.Behaviors.Add(behavior);

			sl.Children.Add(button);

			Assert.NotNull(element);
			Assert.AreSame(button, element);
		}

		[Test]
		public void InvokesCommandWithParameterFromEventArgsPathUsingConverter()
		{
			var sl = new StackLayout();
			var button = new Button { Text = "Some Text" };
			string buttonText = null;
			var behavior = new EventToCommandBehavior
			{
				Command = new Command<string>(t => buttonText = t),
				EventName = "ChildAdded",
				EventArgsParameterPath = "Element",
				EventArgsConverter = new MockButtonTextConverter()
			};

			sl.Behaviors.Add(behavior);

			sl.Children.Add(button);

			Assert.NotNull(buttonText);
			Assert.AreEqual(button.Text, buttonText);
		}
	}
}
