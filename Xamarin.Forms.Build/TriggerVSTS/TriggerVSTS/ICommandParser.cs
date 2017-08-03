using System;
using System.Collections.Generic;

namespace TriggerVSTSBuild
{
	public interface ICommandParser
	{

	}

	public abstract class Command
	{
		public const char COMMAND_SEPARATOR = '|';
		public const char COMMENT_SEPARATOR = ':';
		public const char BUILD_SEPARATOR = ';';
		public const char COMMENT_START = '[';
		public const char COMMENT_END = ']';

		string _rawComment;

		public virtual string Comment {
			get {
				return  _rawComment.Replace (Command.COMMENT_START.ToString (), "").Replace (Command.COMMENT_END.ToString (), "").Trim(); ;
			}
			set {
				_rawComment = value;
			}
		}
		public virtual IList<int> AssociatedBuilds { get; set; }

		public override string ToString ()
		{
			return $"{_rawComment}{COMMENT_SEPARATOR}{string.Join (BUILD_SEPARATOR.ToString (), AssociatedBuilds)}";
		}


	}
}