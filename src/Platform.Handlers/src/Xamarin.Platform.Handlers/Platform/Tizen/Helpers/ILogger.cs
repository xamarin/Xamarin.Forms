namespace Xamarin.Platform.Tizen
{
	public interface ILogger
	{
		void Debug(string tag, string message, string file, string func, int line);

		void Verbose(string tag, string message, string file, string func, int line);

		void Info(string tag, string message, string file, string func, int line);

		void Warn(string tag, string message, string file, string func, int line);

		void Error(string tag, string message, string file, string func, int line);

		void Fatal(string tag, string message, string file, string func, int line);
	}
}

