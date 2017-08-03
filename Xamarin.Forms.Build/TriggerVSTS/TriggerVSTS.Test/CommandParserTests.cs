using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriggerVSTSBuild;

namespace TriggerVSTS.Test
{
	[TestClass]
	public class CommandParserTests
	{
		[TestMethod]
		public void TestParseAllCommands ()
		{
			var parser = new CommandParser ();
			var command1 = "[build]:7022";
			var command2 = "[uitests android]:6766";
			var buildCommands = $"{command1}|{command2}";

			var getCommands = parser.ParseAllCommands (buildCommands);
			Assert.AreEqual (2, getCommands.Count);
			Assert.AreEqual (command1, getCommands [0].ToString());
			Assert.AreEqual (command2, getCommands [1].ToString());
		}

		[TestMethod]
		public void TestParseAllCommandsMultiplebuilds ()
		{
			var parser = new CommandParser ();
			var command1 = "[build]:7022;555;1";
			var command2 = "[uitests android]:6766;5555";
			var buildCommands = $"{command1}|{command2}";

			var getCommands = parser.ParseAllCommands (buildCommands);
			Assert.AreEqual (2, getCommands.Count);
			Assert.AreEqual (command1, getCommands [0].ToString());
			Assert.AreEqual (command2, getCommands [1].ToString ());
			Assert.AreEqual (2, getCommands [1].AssociatedBuilds.Count);
		}

		[TestMethod]
		public void TestParseAllCommandsEmptystringDoesntThrow ()
		{
			var parser = new CommandParser ();
			var buildCommands = $"";

			var getCommands = parser.ParseAllCommands (buildCommands);
			Assert.AreEqual (1, getCommands.Count);		
		}

		[TestMethod]
		public void TestParseAllCommandsJustBuild ()
		{
			var parser = new CommandParser ();
			var command1 = "build";
			var buildCommands = $"{command1}";

			var getCommands = parser.ParseAllCommands (buildCommands);
			Assert.AreEqual (1, getCommands.Count);
		}

		[TestMethod]
		public void TestParseCommentStripBraces ()
		{
			var parser = new CommandParser ();
			var comment = "build";
			var build = 7022;
			var command1 = $"[{comment}]:{build}|";;
			var buildCommands = $"{command1}";

			var getCommands = parser.ParseAllCommands (buildCommands);
			Assert.AreEqual ("build", getCommands[0].Comment);
			Assert.AreEqual (7022, getCommands [0].AssociatedBuilds[0]);
		}

		[TestMethod]
		public void TestParseCommentDefaultIsBuild ()
		{
			var parser = new CommandParser ();
			var comment = "build";
			var getCommands = parser.ParseAllCommands ("");
			Assert.AreEqual (comment, getCommands [0].Comment);
			Assert.AreEqual (null, getCommands [0].AssociatedBuilds);
		}
	}
}
