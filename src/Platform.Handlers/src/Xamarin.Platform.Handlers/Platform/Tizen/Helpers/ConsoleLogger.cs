using System;
using IOPath = System.IO.Path;

namespace Xamarin.Platform.Tizen
{
	internal class ConsoleLogger : ILogger
	{
		public void Debug(string tag, string message, string file, string func, int line)
		{
			Print("D", tag, message, file, func, line);
		}

		public void Verbose(string tag, string message, string file, string func, int line)
		{
			Print("V", tag, message, file, func, line);
		}

		public void Info(string tag, string message, string file, string func, int line)
		{
			Print("I", tag, message, file, func, line);
		}

		public void Warn(string tag, string message, string file, string func, int line)
		{
			Print("W", tag, message, file, func, line);
		}

		public void Error(string tag, string message, string file, string func, int line)
		{
			Print("E", tag, message, file, func, line);
		}

		public void Fatal(string tag, string message, string file, string func, int line)
		{
			Print("F", tag, message, file, func, line);
		}

		void Print(string level, string tag, string message, string file, string func, int line)
		{
			Uri f = new Uri(file);
			Console.WriteLine(
				String.Format(
					"\n[{6:yyyy-MM-dd HH:mm:ss.ffff} {0}/{1}]\n{2}: {3}({4}) > {5}",
					level,  // 0
					tag,  // 1
					IOPath.GetFileName(f.AbsolutePath),  // 2
					func,  // 3
					line,  // 4
					message,  // 5
					DateTime.Now  // 6
				)
			);
		}
	}
}

