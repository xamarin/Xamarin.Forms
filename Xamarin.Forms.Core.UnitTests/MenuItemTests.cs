﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public class MenuItemTests
		: MenuItemTests<MenuItem>
	{
	}

	[TestFixture]
	public abstract class MenuItemTests<T>
		: CommandSourceTests<T>
		where T : MenuItem, new()
	{
		[Test]
		public void Activated()
		{
			var item = new MenuItem();

			bool activated = false;
			item.Clicked += (sender, args) => activated = true;

			item.Activate();

			Assert.That (activated, Is.True);
		}

		[Test]
		public void Command()
		{
			bool executed = false;
			var param = new object();

			var c = new Command (o => {
				Assert.That (o, Is.SameAs (param));
				executed = true;
			});

			var item = new MenuItem { Command = c, CommandParameter = param };
			item.Activate();

			Assert.That (executed, Is.True);
		}

		protected override T CreateSource()
		{
			return new T();
		}

		protected override void Activate (T source)
		{
			source.Activate();
		}

		protected override BindableProperty IsEnabledProperty
		{
			get { return MenuItem.IsEnabledProperty; }
		}

		protected override BindableProperty CommandProperty
		{
			get { return MenuItem.CommandProperty; }
		}

		protected override BindableProperty CommandParameterProperty
		{
			get { return MenuItem.CommandParameterProperty; }
		}
	}
}
