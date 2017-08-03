using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerVSTSBuild
{
	public class CommandParser : ICommandParser
	{

		public IList<Command> ParseAllCommands (string buildCommands)
		{
			List<Command> results = new List<Command> ();
			if (string.IsNullOrEmpty (buildCommands)) {
				results.Add (new MyCommand ("build", null));
				return results;
			}
				

			foreach (var command in buildCommands.Split (Command.COMMAND_SEPARATOR)) {
				var parsedCommand = ParseCommand (command);
				if (parsedCommand != null)
					results.Add (parsedCommand);
			}

			return results;
		}
		public Command ParseCommand (string rawCommands)
		{
			if (string.IsNullOrEmpty (rawCommands))
				return null;
			var parts = rawCommands.Split (Command.COMMENT_SEPARATOR);
			string comment = parts.First ();
			List<int> builds = null;
			ParseBuilds (parts, out builds);

			var cmd = new MyCommand (comment, builds);
			return cmd;
		}

		private void ParseBuilds (string [] parts, out List<int> builds)
		{
			builds = new List<int> ();
			if (parts.Count () > 1) {
				foreach (var item in parts [1].Split (Command.BUILD_SEPARATOR)) {
					builds.Add (int.Parse (item));
				}
			}
			
		}

		class MyCommand : Command
		{
			public MyCommand (string comment, List<int> buildsIds)
			{
				Comment = comment;
				AssociatedBuilds = buildsIds;
			}
		}
	}
}