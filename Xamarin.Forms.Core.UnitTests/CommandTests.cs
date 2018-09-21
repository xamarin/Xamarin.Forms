using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class CommandTests : BaseTestFixture
	{
		
		public void Constructor ()
		{
			var cmd = new Command (() => { });
			Assert.True (cmd.CanExecute (null));
		}

		
		public void ThrowsWithNullConstructor ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command ((Action)null));
		}

		
		public void ThrowsWithNullParameterizedConstructor ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command ((Action<object>)null));
		}

		
		public void ThrowsWithNullCanExecute ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command (() => { }, null));
		}

		
		public void ThrowsWithNullParameterizedCanExecute ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command (o => { }, null));
		}

		
		public void ThrowsWithNullExecuteValidCanExecute ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command (null, () => true));
		}

		
		public void Execute ()
		{
			bool executed = false;
			var cmd = new Command (() => executed = true);

			cmd.Execute (null);
			Assert.True (executed);
		}

		
		public void ExecuteParameterized ()
		{
			object executed = null;
			var cmd = new Command (o => executed = o);

			var expected = new object ();
			cmd.Execute (expected);

			Assert.AreEqual (expected, executed);
		}

		
		public void ExecuteWithCanExecute ()
		{
			bool executed = false;
			var cmd = new Command (() => executed = true, () => true);

			cmd.Execute (null);
			Assert.True (executed);
		}

		
		public void CanExecute ([Values (true, false)] bool expected)
		{
			bool canExecuteRan = false;
			var cmd = new Command (() => { }, () => {
				canExecuteRan = true;
				return expected;
			});

			Assert.AreEqual(expected, cmd.CanExecute (null));
			Assert.True (canExecuteRan);
		}

		
		public void ChangeCanExecute ()
		{
			bool signaled = false;
			var cmd = new Command (() => { });

			cmd.CanExecuteChanged += (sender, args) => signaled = true;

			cmd.ChangeCanExecute ();
			Assert.True (signaled);
		}

		
		public void GenericThrowsWithNullExecute ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command<string> (null));
		}

		
		public void GenericThrowsWithNullExecuteAndCanExecuteValid ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command<string> (null, s => true));
		}

		
		public void GenericThrowsWithValidExecuteAndCanExecuteNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new Command<string> (s => { }, null));
		}

		
		public void GenericExecute ()
		{
			string result = null;
			var cmd = new Command<string> (s => result = s);

			cmd.Execute ("Foo");
			Assert.AreEqual ("Foo", result);
		}

		
		public void GenericExecuteWithCanExecute ()
		{
			string result = null;
			var cmd = new Command<string> (s => result = s, s => true);

			cmd.Execute ("Foo");
			Assert.AreEqual ("Foo", result);
		}

		
		public void GenericCanExecute ([Values (true, false)] bool expected)
		{
			string result = null;
			var cmd = new Command<string> (s => { }, s => {
				result = s;
				return expected;
			});

			Assert.AreEqual (expected, cmd.CanExecute ("Foo"));
			Assert.AreEqual ("Foo", result);
		}

		class FakeParentContext
		{
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		class FakeChildContext
		{
		}

		
		public void CanExecuteReturnsFalseIfParameterIsWrongReferenceType()
		{
			var command = new Command<FakeChildContext>(context => { }, context => true);

			Assert.IsFalse(command.CanExecute(new FakeParentContext()), "the parameter is of the wrong type");
		}

		
		public void CanExecuteReturnsFalseIfParameterIsWrongValueType()
		{
			var command = new Command<int>(context => { }, context => true);

			Assert.IsFalse(command.CanExecute(10.5), "the parameter is of the wrong type");
		}

		
		public void CanExecuteUsesParameterIfReferenceTypeAndSetToNull()
		{
			var command = new Command<FakeChildContext>(context => { }, context => true);

			Assert.IsTrue(command.CanExecute(null), "null is a valid value for a reference type");
		}

		
		public void CanExecuteUsesParameterIfNullableAndSetToNull()
		{
			var command = new Command<int?>(context => { }, context => true);

			Assert.IsTrue(command.CanExecute(null), "null is a valid value for a Nullable<int> type");
		}

		
		public void CanExecuteIgnoresParameterIfValueTypeAndSetToNull()
		{
			var command = new Command<int>(context => { }, context => true);

			Assert.IsFalse(command.CanExecute(null), "null is not a valid valid for int");
		}

		
		public void ExecuteDoesNotRunIfParameterIsWrongReferenceType()
		{
			int executions = 0;
			var command = new Command<FakeChildContext>(context => executions += 1);

			Assert.DoesNotThrow(() => command.Execute(new FakeParentContext()), "the command should not execute, so no exception should be thrown");
			Assert.IsTrue(executions == 0, "the command should not have executed");
		}

		
		public void ExecuteDoesNotRunIfParameterIsWrongValueType()
		{
			int executions = 0;
			var command = new Command<int>(context => executions += 1);

			Assert.DoesNotThrow(() => command.Execute(10.5), "the command should not execute, so no exception should be thrown");
			Assert.IsTrue(executions == 0, "the command should not have executed");
		}

		
		public void ExecuteRunsIfReferenceTypeAndSetToNull()
		{
			int executions = 0;
			var command = new Command<FakeChildContext>(context => executions += 1);

			Assert.DoesNotThrow(() => command.Execute(null), "null is a valid value for a reference type");
			Assert.IsTrue(executions == 1, "the command should have executed");
		}

		
		public void ExecuteRunsIfNullableAndSetToNull()
		{
			int executions = 0;
			var command = new Command<int?>(context => executions += 1);

			Assert.DoesNotThrow(() => command.Execute(null), "null is a valid value for a Nullable<int> type");
			Assert.IsTrue(executions == 1, "the command should have executed");
		}

		
		public void ExecuteDoesNotRunIfValueTypeAndSetToNull()
		{
			int executions = 0;
			var command = new Command<int>(context => executions += 1);

			Assert.DoesNotThrow(() => command.Execute(null), "null is not a valid value for int");
			Assert.IsTrue(executions == 0, "the command should not have executed");
		}
	}
}
