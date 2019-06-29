using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
	public class PromptArguments
	{
		public PromptArguments(string title, string message, string accept, string cancel, string placeholder = null, int? maxLength = null, Keyboard keyboard = default)
		{
			if (string.IsNullOrWhiteSpace(accept) || string.IsNullOrWhiteSpace(cancel))
				throw new Exception($"You must provide a value for {nameof(accept)} and {nameof(cancel)}.");

			Title = title;
			Message = message;
			Accept = accept;
			Cancel = cancel;
			Placeholder = placeholder;
			MaxLength = maxLength;
			Keyboard = keyboard ?? Keyboard.Default;
			PromptResult = new TaskCompletionSource<string>();
		}

		public string Title { get; }

		public string Message { get; }

		public string Accept { get; }

		public string Cancel { get; }

		public string Placeholder { get; }

		public int? MaxLength { get; }

		public Keyboard Keyboard { get; }

		public TaskCompletionSource<string> PromptResult { get; }

		public void SetPromptResult(string text)
		{
			PromptResult.TrySetResult(text);
		}
	}
}